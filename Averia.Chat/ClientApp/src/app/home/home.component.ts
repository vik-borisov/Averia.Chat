import {Component, OnDestroy, OnInit} from '@angular/core';
import {ChatService, MessageDto, UserDto} from "../services/chat.service";
import {UserModalComponent} from "../user-modal/user-modal.component";
import {NgbModal} from "@ng-bootstrap/ng-bootstrap";
import {Subscription} from "rxjs";

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
})
export class HomeComponent implements OnInit, OnDestroy {

  public messages: MessageDto[] = []

  public currentUserId: string

  public users: string[] = []

  subscription: Subscription = new Subscription()

  constructor(public chatService: ChatService,
              private modalService: NgbModal) {
    this.subscription.add(
      chatService.onMessagesChange.subscribe((messages) => this.messages = messages));
    this.subscription.add(
      chatService.onUsersChange.subscribe((users) => this.users = users));
  }

  sendMessage = (text) => {
    this.chatService.sendMessage(text)
  }

  openModal() {
    const modalRef = this.modalService.open(UserModalComponent,
      {
        keyboard: false,
        size: "sm",
        backdrop: "static",
      });
    modalRef.result.then((userId: string) => {
      this.currentUserId = userId

      this.chatService.connectToServer(userId)
      this.chatService.listenServer()
    })
  }

  ngOnInit(): void {
    this.openModal()
  }

  ngOnDestroy(): void {
    this.subscription.unsubscribe()
  }
}
