import {
  ChangeDetectionStrategy,
  Component,
  Input,
  OnInit,
  Output,
  EventEmitter,
  ViewChild,
  ElementRef, AfterViewChecked
} from '@angular/core';
import {MessageDto} from "../services/chat.service";

@Component({
  selector: 'app-chat',
  templateUrl: './chat.component.html',
  styleUrls: ['./chat.component.css'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class ChatComponent implements OnInit, AfterViewChecked {
  @ViewChild('scrollMe') private myScrollContainer: ElementRef;

  public currentMessage: string = ""
  public showPicker: boolean = false

  @Input() messages: MessageDto[];

  @Input() currentUserId: string;

  @Output() newMessage = new EventEmitter<string>();

  constructor() { }

  ngOnInit(): void {
    this.scrollToBottom()
  }

  isCurrentUser = (message: MessageDto) : boolean  => {
    return message.userId == this.currentUserId
  }

  isShowHeader = (currentIndex: number) : boolean  => {
    const message = this.messages[currentIndex]
    return !this.isCurrentUser(message)
      && (currentIndex > 0 && message.userId !== this.messages[currentIndex - 1].userId || currentIndex == 0)
  }

  sendMessage = () => {
    this.newMessage.emit(this.currentMessage)
    this.currentMessage = ""
  }

  ngAfterViewChecked() {
    this.scrollToBottom()
  }

  scrollToBottom = () =>  {
    try {
      this.myScrollContainer.nativeElement.scrollTop = this.myScrollContainer.nativeElement.scrollHeight;
    } catch(err) { }
  }

  togglePicker = () => {
    this.showPicker = !this.showPicker
  }

  addEmoji = (event) => {
    this.currentMessage += event.emoji.native
    this.showPicker = false
  }
}
