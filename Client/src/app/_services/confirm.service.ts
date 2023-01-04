import { Injectable } from '@angular/core';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { Observable } from 'rxjs';
import { ConfirmDialogComponent } from '../modals/confirm-dialog/confirm-dialog.component';

@Injectable({
  providedIn: 'root'
})
export class ConfirmService {
  bsModalRef: BsModalRef;

  constructor(private modalService: BsModalService) { }

  // These are some default values I Can override them anytime I want.
  Confirm(title = 'Confirmation', message = 'Are you sure you want to do this.!', btnOkText = 'OK',
    btnCancelText = 'Cancel'): Observable<boolean> { // So anywhere will user this need to subscribe to get the result
    const config = {
      initialState: {
        title,
        message,
        btnOkText,
        btnCancelText
      }
    }
    this.bsModalRef = this.modalService.show(ConfirmDialogComponent, config);

    return new Observable<boolean>(this.GetResult());
  }

  private GetResult() {
    return (observer) => {
      const subscription = this.bsModalRef.onHidden.subscribe(() => {
        observer.next(this.bsModalRef.content.result);
        observer.complete();
      });
      return {
        unsubscribe() {
          subscription.unsubscribe();
        }
      }
    }
  }
}
