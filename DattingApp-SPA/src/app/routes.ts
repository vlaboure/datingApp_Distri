import {Routes} from '@angular/router';
import {HomeComponent} from './home/home.component';
import {MemberListComponent} from './members/member-list/member-list.component';
import {MessagesComponent} from './messages/message.component';
import {ListsComponent} from './lists/lists.component';
import { AuthGuard } from './_guards/auth.guard';
import { MemberDetailComponent } from './members/member-detail/member-detail.component';
import { MemberDetailResolver } from './_resolver/member-detail-resolver';
import { MemberListResolver } from './_resolver/member-list-resolver';
import { MemberEditComponent } from './members/member-edit/member-edit.component';
import { MemberEditResolver } from './_resolver/member-edit-resolver';
import { PreventUnsavedChanges } from './_guards/prevent-unsaved-changes';
import { ListResolver } from './_resolver/list-resolver';
import { MessagesResolver } from './_resolver/messages-resolver';


// tableau de routes
export const appRoutes: Routes = [
    {path : '', component: HomeComponent},
    {   path : '',
        runGuardsAndResolvers: 'always',
        canActivate: [AuthGuard],
        children:[
            {path : 'members', component: MemberListComponent, 
                resolve: {users: MemberListResolver}},
            {path : 'messages', component: MessagesComponent,
                resolve: {messages: MessagesResolver}},
            {path : 'members/edit', component: MemberEditComponent,
                    resolve: {user: MemberEditResolver}, canDeactivate: [PreventUnsavedChanges]},
            {path : 'lists', component: ListsComponent,
                resolve : {users : ListResolver}},
            // appel du resolver dans la route pour récuperer les datas du user
            {path : 'members/:id', component : MemberDetailComponent, resolve: {user: MemberDetailResolver}}
        ]
    },       // canActivate [tableau de guards]

    // pathMatch: 'full' pour correspondance totale
   {path : '**', redirectTo: 'home', pathMatch: 'full'}
]