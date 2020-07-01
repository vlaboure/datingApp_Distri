import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/Http';
import { Observable } from 'rxjs';
import { User } from '../_models/User';
import { PaginatedResult } from '../_models/pagination';
import { map } from 'rxjs/operators';
import { JsonPipe } from '@angular/common';
import { Message } from '../_models/message';

/**
 * création d'un header pour autorisations pour les requêtes get
 */
    /* plus nécessaire car  JwtModule.forRoot dans app.module*/
// const httpOptions = {
//   headers: new HttpHeaders({
//     // tslint:disable-next-line: object-literal-key-quotes
//     'Authorization' : 'Bearer ' + localStorage.getItem('token')
//   })
// };

@Injectable({
  providedIn: 'root'
})
export class UserService {
  baseUrl = environment.apiUrl;
constructor(private http: HttpClient) { }

// retourne un objet contenant le result--> contenu passé dans la requete
                            // l'interface pagination-->
                            /*currentPage: number;
                              itemsPerPage: number;
                              totalItems: number;
                              totalPages: number; */

  getUsers(page?, itemsPerPage?, userParams?,likesParam?): Observable<PaginatedResult<User[]>>{
    // il faut typer le retrun <User[]>car get retourne un object et pas un user
    // si pas de  JwtModule.forRoot dans app.module--> get doit contenir option pour token
    const paginatedResult: PaginatedResult<User[]> = new PaginatedResult<User[]>();
    let params = new HttpParams();
    if(page!= null && itemsPerPage!= null){
      // pageNumber de params.cs
      params = params.append('pageNumber',page);
      // pageSize de params.cs
      params = params.append('pageSize',itemsPerPage);
    }

    if(userParams != null){
      params = params.append('minAge', userParams.minAge);
      params = params.append('maxAge', userParams.maxAge);
      params = params.append('gender', userParams.gender);
    }

    if(likesParam === 'Likers'){
      params = params.append('Likers', 'true');
    }  
    if(likesParam === 'Likees'){
      params = params.append('Likees', 'true');
    }  
    // environment.apiUrl--> 'http://localhost:5000/api/'
    return this.http
    .get<User[]>(environment.apiUrl + 'users', {observe: 'response',params})
    .pipe(
      // map en retour :
      // on récupère le header --> paginatedResult.result = response.body
      //                       --> paginatedResult.pagination = JSON....      
      map(response=>{
        paginatedResult.result = response.body;
        if (response.headers.get('Pagination') != null){
          paginatedResult.pagination = JSON.parse(response.headers.get('Pagination'));
        }
        return paginatedResult;
      })
    );
  }

  /**
   * Méthode pour récupérer les messages avec pagination
   */
  getMessages(id: number, page?, itemsPerPage?, contener?){
    // comme pour getUsers on récupère les info de pagination et le message
    const paginatedResult: PaginatedResult<Message[]> = new PaginatedResult<Message[]>();

    let params = new HttpParams();

    params = params.append('Contener', contener);

    if (page != null && itemsPerPage != null){
      params = params.append('pageNumber', page);
      params = params.append('pageSize', itemsPerPage);
    }
    return this.http
      .get<Message[]>(environment.apiUrl + 'users/' + id + '/messages',{
        observe: 'response',params})
      .pipe( 
        map(response => {
          paginatedResult.result = response.body;
          if (response.headers.get('Pagination') !== null){
            paginatedResult.pagination = JSON.parse(response.headers.get('Pagination'));
          }
          return paginatedResult;
        })
      );
  }

  getMessagesThread(id: number, receptId: number){
    return this.http
      .get<Message[]>(environment.apiUrl + 'users/' + id + '/messages/thread/' + receptId);
  }

  getUser(id: number): Observable<User>{
    // il faut typer le retrun <User[]>car get retourne un object et pas un user
        // si pas de  JwtModule.forRoot dans app.module--> get doit contenir option pour token
    return this.http.get<User>(environment.apiUrl + 'users/' + id);
  }
  
  updateUser(id: number,user: User){
    return this.http.put(this.baseUrl + 'users/' + id, user);
  }

  setMainPhoto(userId: number, id: number){
    return this.http.post(this.baseUrl + 'users/' + userId + '/photos/' + id + '/setMain', {});
  }

  deletePhoto(userId: number, id: number){
    return this.http.delete(this.baseUrl + 'users/' + userId + '/photos/' + id);
  }

  sendLike(id: number, receptId : number){
    return this.http.post(this.baseUrl + 'users/' + id + '/like/' + receptId, {});
  }

  sendMessage(id: number, message: Message){
    return this.http.post(this.baseUrl + 'users/' + id + '/messages', message);
  }

  deleteMessage(id: number, userId: number){
    return this.http.post(this.baseUrl + 'users/' + userId + '/messages/' + id,{});
  }
  markMessageRead(userId: number, id: number){
    // on souscrit au service directement dans service car il n'y a rien à renvoyer
    this.http.post(this.baseUrl + 'users/' + userId + '/messages/' + id + '/read',{}).subscribe();
  }
}
