import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { FileUploader } from 'ng2-file-upload';
import { Photo } from 'src/app/_models/photo';
import { environment } from 'src/environments/environment';
import { AuthService } from 'src/app/_services/auth.service';
import { UserService } from 'src/app/_services/user.service';
import { AlertifyService } from 'src/app/_services/alertify.service';




// child de member-edit.component
@Component({
  selector: 'app-photo-editor',
  templateUrl: './photo-editor.component.html',
  styleUrls: ['./photo-editor.component.css']
})
export class PhotoEditorComponent implements OnInit {
  @Input() photos: Photo[];
  @Output() getMemeberPhotoChange = new EventEmitter<string>();
  uploader: FileUploader;
  hasBaseDropZoneOver = false;
  baseUrl = environment.apiUrl;
  currentMain: Photo;

  constructor(private authService: AuthService, private userServices: UserService, 
              private alertifyService: AlertifyService) {}

  ngOnInit() {
    this.initializeUploader();
  }
  
  public fileOverBase(e: any): void {
    this.hasBaseDropZoneOver = e;
  }
  // fonction pour initialiser le composant

  initializeUploader(){
    this.uploader = new FileUploader({
      url : this.baseUrl + 'users/' +  this.authService.decodedToken.nameid + '/photos',
      authToken : 'Bearer ' + localStorage.getItem('token'),
      isHTML5: true,
      allowedFileType: ['image'],
      removeAfterUpload: true,
      autoUpload: false,
      maxFileSize: 10 * 1024 * 1024
    });

   // parametrage du credential dans la methode onAfterAddingFile
   // la photo telechargée en local n'utilisera pas les cookies
    this.uploader.onAfterAddingFile = (file) => {file.withCredentials = false; };

                // rafraichissement de la galerie
    // après chargement dans le cloud et envoi du PublicId à la base par le cloud
    // on charge les données depuis
    // onSuccessItem renvoyé par FileUploader quand la photo est correctement uploadée
    this.uploader.onSuccessItem = (item, response, status, headers) => {
      if (response){
        const res: Photo = JSON.parse(response);
        const photo = {
          id: res.id,
          dateAdded: res.dateAdded,
          url: res.url,
          description: res.description,
          isMain: res.isMain
        };
        // ajout de la photo a l'array
        this.photos.push(photo);
        // si le user vient d'être créé, la photo est ajoutée au tableau mais
        // le rafraichissement ne se fait que lors du chargement depuis le serveur et la BDD
        // donc si la photo est main on fait un refersh  pour afficher immédiatement la photo
        if (photo.isMain){
          // même chose que dans setMainPhoto
          this.authService.changeMemberPhoto(photo.url);
          this.authService.currentUser.photoUrl = photo.url;
          localStorage.setItem('user', JSON.stringify(this.authService.currentUser));
        } 
      }
    };
  }

  setMainPhoto(photo: Photo){
      this.userServices.setMainPhoto(this.authService.decodedToken.nameid , photo.id).subscribe(() => {
      // on change le main dans la partie angular
      // le but est d'afficher l'image sans interroger le serveur
      // reste à rafraichir le parent--> member-edit.component
      this.currentMain = this.photos.filter(p => p.isMain === true)[0];
      this.currentMain.isMain = false;
      photo.isMain = true;
      this.authService.changeMemberPhoto(photo.url);
      // pour le changement de photo et le rafraichissement
      // changé et this.getMemeberPhotoChange.emit(photo.url) n'est plus utilisé remplacé avec le auth
      this.authService.currentUser.photoUrl = photo.url;
      localStorage.setItem('user', JSON.stringify(this.authService.currentUser));
      //**************************** */
      // this.getMemeberPhotoChange.emit(photo.url); changé avec le behaviorSubject
    }, error => { this.alertifyService.error('erreur lors de la tentative de changement de main photo')})
  }

  deletePhoto(id: number){
    this.alertifyService.confirm('sur de vouloir supprimer la photo', () => {
                                         //.authService.decodedToken.nameid  
      this.userServices.deletePhoto(this.authService.currentUser.id, id).subscribe(() => {
        this.photos.splice(this.photos.findIndex( p => p.id === id), 1);
        this.alertifyService.success('photo supprimée avec succès');  
      }, error => {this.alertifyService.error('erreur de suppression '+ error)});
    })
  }
}
