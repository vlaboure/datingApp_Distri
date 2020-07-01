import { BrowserModule, HammerGestureConfig, enableDebugTools, HAMMER_GESTURE_CONFIG } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { TimeagoModule } from 'ngx-timeago';
import { PaginationModule } from 'ngx-bootstrap/pagination';
import {BsDatepickerModule} from 'ngx-bootstrap/datepicker';
import { HttpClientModule } from '@angular/common/Http';
import { JwtModule } from '@auth0/angular-jwt'; // a mettre en tête
import {FormsModule, ReactiveFormsModule} from '@angular/forms';
import { RouterModule } from '@angular/router';
import { FileUploadModule } from 'ng2-file-upload';
import { NgxGalleryModule } from '@kolkov/ngx-gallery';
import { AppComponent } from './app.component';
import { NavComponent } from './nav/nav.component';
import { AuthService } from './_services/auth.service';
import { HomeComponent } from './home/home.component';
import { RegisterComponent } from './register/register.component';
import { ErrorInterceptorProvider } from './_services/error.Interceptor';
import { BsDropdownModule} from 'ngx-bootstrap/dropdown';
import { TabsModule } from 'ngx-bootstrap/tabs';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { MemberListComponent } from './members/member-list/member-list.component';
import { ListsComponent } from './lists/lists.component';
import { MessagesComponent } from './messages/message.component';
import {AuthGuard} from './_guards/auth.guard';
import { appRoutes } from './routes';
import {UserService} from './_services/user.service';
import { ButtonsModule } from 'ngx-bootstrap/buttons';
import { MemberCardComponent } from './members/member-card/member-card.component';
import { MemberDetailComponent } from './members/member-detail/member-detail.component';
import { AlertifyService } from './_services/alertify.service';
import { MemberDetailResolver } from './_resolver/member-detail-resolver';
import { MemberListResolver } from './_resolver/member-list-resolver';
import { MemberEditComponent } from './members/member-edit/member-edit.component';
import { MemberEditResolver } from './_resolver/member-edit-resolver';
import { PreventUnsavedChanges } from './_guards/prevent-unsaved-changes';
import { PhotoEditorComponent } from './members/photo-editor/photo-editor.component';
import { ListResolver } from './_resolver/list-resolver';
import { MessagesResolver } from './_resolver/messages-resolver';
//import { TimeAgoObsPipe } from './_services/TimeAgoObsPipe';
import { ResetComponent } from './reset/reset.component';
import { MemberMessagesComponent } from './members/member-messages/member-messages.component';


export function tokenGetter(){
   return localStorage.getItem('token');
}

export class CustomHammerConfig extends HammerGestureConfig{
   overrides = {
      pinch: {enable: false},
      rotate: {enable: false}
   };
}


@NgModule({
   declarations: [
      AppComponent,
      NavComponent,
      HomeComponent,
      RegisterComponent,
      MemberListComponent,
      MemberMessagesComponent,
      ListsComponent,
      MessagesComponent,
      MemberCardComponent,
      MemberDetailComponent,
      MemberEditComponent,
      PhotoEditorComponent,
      //TimeAgoObsPipe,
      ResetComponent
   ],

   imports: [
      /******utilier.froRootpermetdecréerunsingleton\n*****pourn'appliquerleservicesurlarouteencours\n*/
      TimeagoModule.forRoot(),
      HttpClientModule,
      NgxGalleryModule,
      FormsModule,
      BrowserAnimationsModule,
      BsDatepickerModule.forRoot(),
      BrowserAnimationsModule,
      BsDropdownModule.forRoot(),
      TabsModule.forRoot(),
      PaginationModule.forRoot(),
      ButtonsModule.forRoot(),
      ReactiveFormsModule,
      RouterModule.forRoot(appRoutes),
      TimeagoModule.forRoot(),
      FileUploadModule,
      JwtModule.forRoot({
         config: {
           tokenGetter,
           whitelistedDomains: ['localhost:5000'],
           blacklistedRoutes: ['localhost:5000/api/auth']
         }
       })
   ],
   
   bootstrap: [AppComponent],

   providers: [
      ErrorInterceptorProvider,
      AuthService,
      AlertifyService,
      UserService,
      MemberDetailResolver,
      MemberListResolver,
      MemberEditResolver,
      MessagesResolver,
      ListResolver,
      PreventUnsavedChanges,
      {provide: HAMMER_GESTURE_CONFIG, useClass: CustomHammerConfig}
   ]
})
export class AppModule { }
