import { Component, OnInit } from '@angular/core';
import { User } from '../../_models/User';
import { AlertifyService } from '../../_services/alertify.service';
import { UserService } from '../../_services/user.service';
import { Route } from '@angular/compiler/src/core';
import { ActivatedRoute } from '@angular/router';
import { Pagination, PaginatedResult } from 'src/app/_models/pagination';

@Component({
  selector: 'app-member-list',
  templateUrl: './member-list.component.html',
  styleUrls: ['./member-list.component.css']
})
export class MemberListComponent implements OnInit {
  users: User[];
  // récpération du user logué
  user: User = JSON.parse(localStorage.getItem('user'));
  // tableau de deux valeurs 
  genderList = [{value : 'male', display : 'Males'},{value : 'female', display : 'Females'}];
  userParams: any = {};
  pagination: Pagination;

  constructor(private userService: UserService, private alertifyService: AlertifyService
            , private route: ActivatedRoute) { }

  ngOnInit() {
    // this.loadUSers();
    this.route.data.subscribe(data => {
      // ['users'] correspond à la propriété user: dans route.ts
      //  {path : 'members', component: MemberListComponent, resolve: {users: MemberListResolver}},
      this.users = data['users'].result;
      this.pagination = data['users'].pagination;
    });
    this.setFilters();
  }
  setFilters(){
    this.userParams.gender = this.user.gender === 'female' ? 'male' : 'female';
    this.userParams.minAge = 18;
    this.userParams.maxAge = 99;
  }

  resetFilters(){
    this.setFilters();
    this.loadUsers();
  }

  pageChanged(event: any): void {
    // page--> propriété de angular html
    this.pagination.currentPage = event.page;
    this.loadUsers();
  }
    // appel de l'observable getUsers

  loadUsers(){
    this.userService
      .getUsers(this.pagination.currentPage, this.pagination.itemsPerPage, this.userParams)
      .subscribe(
        (res: PaginatedResult<User[]>) =>{
          // res.result = body http
          this.users = res.result;
          this.pagination = res.pagination;
          }, error =>{
            this.alertifyService.error(error);
          }
      );
  }
}
