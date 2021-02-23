import { Pipe, PipeTransform } from '@angular/core';
import { Route } from '../models/route.model';

@Pipe({
    name: 'filter'
})

export class FilterPipe implements PipeTransform {
    transform(array: Route[], startWith: string): any {
        let temp: Route[] = [];

        if (array != undefined && array != null)
            temp = array.filter(a => a.Route.indexOf(startWith) != -1 || a.RedirectUrl.indexOf(startWith) != -1 || a.PageTitle.indexOf(startWith) != -1);

        return temp;
    }
}