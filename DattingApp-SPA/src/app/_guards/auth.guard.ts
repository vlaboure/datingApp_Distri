import { Injectable } from '@angular/core';
import { CanActivate, Router } from '@angular/router';
import { AlertifyService } from '../_services/alertify.service';
import { AuthService } from '../_services/auth.service';

// composant permettant de contrôler et restreindre les accés aux routes
@Injectable({
  providedIn: 'root'
})
export class AuthGuard implements CanActivate {
  // dans le constructeur on crée des instances des classes à utiliser pour contrôler l'user
  constructor(
    private authService: AuthService,
    private router: Router,
    private alertify: AlertifyService
  ){

  }
          // ********** appel de base avec ng g ruard auth */
  // canActivate(
  //   next: ActivatedRouteSnapshot,
  //   state: RouterStateSnapshot): Observable<boolean | UrlTree> | Promise<boolean | UrlTree> | boolean | UrlTree {
          // on n'a besoin que de savoir si l'user est connecté donc un boolean
  canActivate(): boolean{
    // loggé ? ok
    if (this.authService.loggedIn()){
      return true;
    }
    // non loggé message erreur
    this.alertify.error('dégage de là gros pédé');
    // redirection vers accueil
    this.router.navigate(['/home']);
    return false;
  }
}
