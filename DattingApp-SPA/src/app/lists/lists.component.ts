import { Component, OnInit } from '@angular/core';
import { User } from '../_models/User';
import { Pagination, PaginatedResult } from '../_models/pagination';
import { UserService } from '../_services/user.service';
import { AlertifyService } from '../_services/alertify.service';
import { ActivatedRoute } from '@angular/router';
import { AuthService } from '../_services/auth.service';

@Component({
  selector: 'app-lists',
  templateUrl: './lists.component.html',
  styleUrls: ['./lists.component.css']
})
export class ListsComponent implements OnInit {
  users: User[];
  pagination: Pagination;
  likesParam: string;
  constructor(private userService: UserService, private alertifyService: AlertifyService
    ,private route: ActivatedRoute) { }

  ngOnInit() {
    this.route.data.subscribe(data => {
      // ['users'] correspond à la propriété user: dans route.ts
      //  {path : 'members', component: MemberListComponent, resolve: {users: MemberListResolver}},
      this.users = data['users'].result;
      this.pagination = data['users'].pagination;
    });

    this.likesParam = 'Likers';
  }

  loadUsers(){
      this.userService
      .getUsers(this.pagination.currentPage, this.pagination.itemsPerPage, null, this.likesParam)
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

  pageChanged(event: any): void {
    this.pagination.currentPage = event.page;
    this.loadUsers();
  }
}
