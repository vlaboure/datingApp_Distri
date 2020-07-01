import { Component, OnInit,Input, Output, EventEmitter } from '@angular/core';
import { AuthService } from '../_services/auth.service';
import { BsDatepickerConfig } from 'ngx-bootstrap/datepicker';
import { AlertifyService } from '../_services/alertify.service';
import { FormGroup, FormControl, Validators, FormBuilder } from '@angular/forms';
import { User } from '../_models/User';
import { Router } from '@angular/router';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit {
 // @Input() valuesFromHome;
    // communication enfant -->parent
  @Output() cancelRegister = new EventEmitter(); 
  user: User;
  registerForm: FormGroup;
    // déclaration de bsConfig en partial pour redre tous les paramètres optionne
  bsConfig: Partial <BsDatepickerConfig>;
  constructor(
    public authService: AuthService,
    private alertify: AlertifyService,
    private fb: FormBuilder,
    private router: Router
  ) { }

  ngOnInit() {
      // appliquer la couleur au datepicker
    this.bsConfig = {
      containerClass: 'theme-red'
    },
    
      // remplace la methode register initiale ET REMPLACE PAR 
      // FormBuilder 
    // this.registerForm = new FormGroup({
    //   username: new FormControl('',Validators.required),
    //   password: new FormControl('',[Validators.required,Validators.minLength(6),
    //                 Validators.maxLength(12)]),
    //   confirmpassword: new FormControl('', Validators.required) 
    // },this.passwordMatchValidator);
    /*********Remplace toutes les entrées dessus :*****/
    this.createForm();
  }

  createForm(){
    this.registerForm = this.fb.group({
      gender:['male'],
      username: ['', Validators.required],
      knownAs: ['', Validators.required],
      dateOfBirth: [null, Validators.required],
      city: ['', Validators.required],
      country: ['', Validators.required],
      password: ['', [Validators.required,Validators.minLength(6),
        Validators.maxLength(12)]],
      confirmpassword: ['', Validators.required]
    }, { validators: this.passwordMatchValidator});
  }

  passwordMatchValidator(g: FormGroup){
    return g.get('password').value === g.get('confirmpassword').value ? null : {'mismatch': true } 
  }

  register(){
    // this.authService.register(this.model).subscribe(() => {
    //   this.alertify.success('register accompli');
    // }, error => {this.alertify.error(error); });

        //***   register avec formGroup */
    if (this.registerForm.valid){
        // assign: méthode js 
        // copie les données d'un objet dans un autre {}
      this.user = Object.assign({}, this.registerForm.value);

      // souscription à register de authService
      this.authService.register(this.user).subscribe(() => {
        this.alertify.success('register réussi');
      }, error => 
      { this.alertify.error(error); },
      // dernier paramètre de subscribe--> complete (complément)
      // on y met le login et une redirection vers la page d'accueil
      () => {
          this.authService.login(this.user).subscribe(() => {
          this.router.navigate(['/members']);
          });
        }
      );
    }
  }

  cancel(){
    // envoi de l'event avec la valeur false pour valider le cancel
    this.cancelRegister.emit(false);
    console.log('cancelled');
  }
}
