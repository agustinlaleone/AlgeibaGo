import { Component, Injector } from '@angular/core';
import { Injectable} from '@angular/core';

@Component({
  selector: 'app-errors-handler',
  templateUrl: './errors-handler.component.html',
  styleUrls: ['./errors-handler.component.css']
})

@Injectable()
export class ErrorsHandlerComponent {

  constructor(private injector: Injector) { }

}