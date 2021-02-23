import { Component, Injector } from '@angular/core';
import { Injectable} from '@angular/core';

@Component({
  selector: 'app-servererror',
  templateUrl: './servererror.component.html',
  styleUrls: ['./servererror.component.css']
})
@Injectable()
export class ServererrorComponent {

  constructor(private injector: Injector) { }

}