import { Injectable } from "@angular/core";
import { MemberDetailComponent } from '../members/member-detail/member-detail.component';
import { CanDeactivate } from '@angular/router';
import { MemberEditComponent } from '../members/member-edit/member-edit.component';

@Injectable()
export class PreventUnsavedChanges implements CanDeactivate<MemberEditComponent>{
    canDeactivate(component: MemberEditComponent){
        if (component.editForm.dirty){
            return confirm('Etes vous sur de vouloir quitter ? tous les changements seront perdus');
        }
        return true;
    }
}