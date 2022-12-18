import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  selector: 'app-server-error',
  templateUrl: './server-error.component.html',
  styleUrls: ['./server-error.component.css']
})
export class ServerErrorComponent implements OnInit {
  error: any;
  constructor(private router: Router) {
    // We can only access the router state inside the constructor
    const navigation = this.router.getCurrentNavigation();
    // We will use safe navigation operator (optional chaining operator '?') 
    //because when the user refresh the page then we will immediatly lose whatever inside the navigation state
    // We only will get it once when we redirect the user to this page.
    this.error = navigation?.extras?.state?.error;
  }

  ngOnInit(): void {
  }

}
