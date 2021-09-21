import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';
import { RouterModule } from '@angular/router';

import { AppComponent } from './app.component';
import { HomeComponent } from './home/home.component';
import {UserModalComponent} from "./user-modal/user-modal.component";
import { ChatComponent } from './chat/chat.component';
import { UsersComponent } from './users/users.component';
import { PickerModule } from '@ctrl/ngx-emoji-mart';
import {EmojiModule} from "@ctrl/ngx-emoji-mart/ngx-emoji";

@NgModule({
  declarations: [
    AppComponent,
    HomeComponent,
    UserModalComponent,
    ChatComponent,
    UsersComponent
  ],
  imports: [
    BrowserModule.withServerTransition({appId: 'ng-cli-universal'}),
    HttpClientModule,
    PickerModule,
    FormsModule,
    RouterModule.forRoot([
      {path: '', component: HomeComponent, pathMatch: 'full'},
    ]),
    EmojiModule
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
