import {Component, OnInit} from '@angular/core';
import { NgbActiveModal} from '@ng-bootstrap/ng-bootstrap';

export class UserModel {
  public name: string;
}

@Component({
  selector: 'app-user-modal',
  templateUrl: './user-modal.component.html',
  styleUrls: ['./user-modal.component.css']
})
export class UserModalComponent implements OnInit {
  model = new UserModel();

  constructor(public activeModal: NgbActiveModal) {}

  ngOnInit(): void {
  }

  onSubmit() {
    this.activeModal.close(this.model.name)
  }
}
