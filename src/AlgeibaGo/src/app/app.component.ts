import { Component } from '@angular/core';
import { ParamMap, Router, ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent {
  
  constructor(private route: ActivatedRoute, private router: Router) {}

  parameters: any;

  ngOnInit() {
    
  } 
}