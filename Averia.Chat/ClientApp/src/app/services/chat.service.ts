import {EventEmitter, Injectable} from '@angular/core';
import * as signalR from "@microsoft/signalr";

export interface MessageDto {
  text: string,
  userId: string
  createDate: number
}

export enum EUserStatus {
  join = 10,
  left = 20
}

export interface UserDto {
  userId: string
  status: EUserStatus
}


@Injectable({
  providedIn: 'root'
})
export class ChatService {
  private messagesMap: { string: MessageDto } = {} as { string: MessageDto };
  private usersMap: { string: boolean } = {} as { string: boolean };

  private hubConnection: signalR.HubConnection

  public onMessagesChange: EventEmitter<MessageDto[]> = new EventEmitter<MessageDto[]>()
  public onUsersChange: EventEmitter<string[]> = new EventEmitter<string[]>()

  constructor() {
  }

  public connectToServer = (username: string) => {
    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl(`/chat?userId=${username}`)
      .configureLogging(signalR.LogLevel.Information)
      .withAutomaticReconnect()
      .build();

    this.hubConnection
      .start()
      .then(() => console.log('Connection started'))
      .catch(err => console.log('Error while starting connection: ' + err))

    this.hubConnection
      .onclose(err => {
        this.usersMap = {} as { string: boolean }
        this.onUsersChange.emit([])
      })
  }

  public listenServer = () => {
    this.hubConnection.on('ReceiveMessages', (messages: MessageDto[]) => {
      for (const message of messages) {
        this.messagesMap[message.createDate] = message;
      }
      this.onMessagesChange.emit(Object.values(this.messagesMap))
    });

    this.hubConnection.on('UsersUpdate', (users: UserDto[]) => {
      for (const user of users) {
        switch (user.status) {
          case EUserStatus.join:
            this.usersMap[user.userId] = true
            break;
          case EUserStatus.left:
            if (user.userId in this.usersMap) {
              delete this.usersMap[user.userId]
            }
            break;
        }
      }

      this.onUsersChange.emit(Object.keys(this.usersMap).sort())
    });
  }

  public sendMessage = (text: string) => {
    if (!text) {
      return
    }

    this.hubConnection.invoke('SendMessage', text).catch(err => console.error(err))
  }
}
