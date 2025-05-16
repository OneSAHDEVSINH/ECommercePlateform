import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MessageService, Message } from '../../services/general/message.service';

@Component({
  selector: 'app-message',
  standalone: true,
  templateUrl: './message.component.html',
  styleUrl: './message.component.css',
  imports: [CommonModule]
})
export class MessageComponent implements OnInit {
  message: Message | null = null;

  constructor(private messageService: MessageService) { }

  ngOnInit(): void {
    this.messageService.currentMessage.subscribe(message => {
      this.message = message;
    });
  }

  closeMessage(): void {
    this.messageService.clearMessage();
  }
}
