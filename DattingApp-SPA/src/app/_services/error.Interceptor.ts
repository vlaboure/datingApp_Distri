import { Injectable } from '@angular/core';
import { HttpInterceptor, HttpErrorResponse, HTTP_INTERCEPTORS } from '@angular/common/Http';
import { catchError } from 'rxjs/operators';
import { throwError } from 'rxjs';

@Injectable()
export class ErrorInterceptor implements HttpInterceptor{
    // implementation de l'interface pour Httpinterceptor
    intercept(req: import('@angular/common/Http').HttpRequest<any>, next: import('@angular/common/Http')
    .HttpHandler): import('rxjs')
    .Observable<import('@angular/common/Http').HttpEvent<any>> {
        // throw new Error('Method not implemented.');
        return next.handle(req).pipe(
            catchError(error =>{
                if (error instanceof HttpErrorResponse){
                    if (error.status === 401){
                        return throwError(error.statusText);
                    }
                    const applicationError = error.headers.get('Application-Error');
                    if (applicationError){
                        return throwError(applicationError);
                    }
                    const serverError = error.error;
                    let modalStateError = '';
                       // errors est uen propriété de l'objet error retourné par le serveur api
                    if(serverError.errors && typeof serverError.errors ==='object'){
                        for(const key in serverError.errors){
                            if(serverError.errors[key] ){
                                modalStateError += serverError.errors[key] + '\n';
                            }
                        }
                    }  
                    return throwError(modalStateError || serverError || 'Autre chose problème...')
                }
            })
        );
    }
}

export const ErrorInterceptorProvider = {
    provide : HTTP_INTERCEPTORS,
    useClass : ErrorInterceptor,
    multi : true
};
