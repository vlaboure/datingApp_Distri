import {Injectable} from '@angular/core';
import {User} from '../_models/user';
import { UserService } from '../_services/user.service';
import {Resolve, Router, ActivatedRouteSnapshot} from '@angular/router';
import { AlertifyService } from '../_services/alertify.service';
import { Observable, of } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { AuthService } from '../_services/auth.service';

@Injectable()
export class MemberEditResolver implements Resolve<User>{
   constructor(private userService: UserService, private router: Router, private authService: AuthService,
               private alertify: AlertifyService){}
               
   resolve(route: ActivatedRouteSnapshot): Observable<User>{
      /** le
       * new Claim(ClaimTypes.NameIdentifier, userFromRepo.Id.ToString()),
       * de login est enregistré comme nameid dans la requete de token
       */
      return this.userService.getUser(this.authService.decodedToken.nameid).pipe(
         catchError(error => {
            this.alertify.error('Erreur lors de la récupération vos datas');
            this.router.navigate(['/members']);
            return of(null); // return of--> return observable of(null)
         })
      );
   }
}