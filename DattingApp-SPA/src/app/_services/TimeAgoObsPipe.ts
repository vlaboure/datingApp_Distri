import { OnDestroy, ChangeDetectorRef, Pipe, PipeTransform } from '@angular/core';
import { AsyncPipe } from '@angular/common';
import { Observable, interval, of, timer, observable} from 'rxjs';
import { repeatWhen, startWith, takeWhile, mergeMap, map } from 'rxjs/operators';

@Pipe({
  name: 'timeAgoObs',
  pure: false
})
export class TimeAgoObsPipe implements PipeTransform, OnDestroy {
  private readonly async: AsyncPipe;

  private isDestroyed = false;
  private value: Date;
  private timer: Observable<string>;

  // On récupère une instance de ChangeDetectorRef, 
  // afin de contrôler nous-mêmes la stratégie de détection de changement à appliquer
  constructor(ref: ChangeDetectorRef) {
    this.async = new AsyncPipe(ref);
  }

  public transform(sinceDate: string): string {

    // On vérifie la valeur d'entrée du pipe, qui doit être une chaîne de caractères représentant une date
    // if(new Date(sinceDate).toString() === "Invalid Date" || Date.parse(sinceDate) === NaN) {
    //   throw new Error('Ce pipe ne fonctionne qu’avec des chaînes de caractères représentant des dates');
    // }

    // On convertit la valeur d’entrée du pipe en date JavaScript
    this.value = new Date(sinceDate);

    if (!this.timer) {
      this.timer = this.getObservable(); // On initialise le Timer de notre pipe
    }

    // On retourne un pipe asynchrone, avec les valeurs émises par notre Timer.
    return this.async.transform(this.timer); 
  }

  // Fonction outil permettant de renvoyer la date courante 
  public now(): Date {
    return new Date();
  }

  // On évite les fuites de mémoire au sein de notre pipe, en se désabonnant à notre Timer
  public ngOnDestroy() {
    // On met fin au Timer, au prochain intervalle de temps, le Timer émettra un événement de type ‘complete’
    this.isDestroyed = true; 
  }

  private getObservable() {
    // On crée un flux contenant le nombre 1.
    return of(1).repeatWhen(notifications => {
        // On émet un événement à certains intervalles de temps :
        return notifications.mergeMap((x, i) => {

          // si le temps écoulé est inférieur à soixante secondes, on met à jour l’interface toutes les secondes,
          // sinon on met à jour l’interface toutes les trente secondes
          const sleep = i < 60 ? 1000 : 30000;

          //  Après que le temps ci-dessus soit écoulé, on émet un événement
          return timer(sleep);
        });
      })
      // On émet un événement tant que la méthode ngOnDestroy n’a pas été appelée
      .takeWhile(_ => !this.isDestroyed)
      // On renvoie une chaîne de caractères indiquant le temps écoulé, grâce à la méthode elapsed
      .map((x, i) => this.elapsed());
  };

  // On détermine la chaîne de caractères à émettre en fonction du temps écoulé 
  // depuis la date passée en paramètre du pipe
  private elapsed(): string {
    // On récupère la date actuelle pour calculer le temps écoulé
    let now = this.now().getTime();

    // On calcule le delta en secondes entre la date actuelle et la date passée en paramètre du pipe
    let delta = (now - this.value.getTime()) / 1000;

    // On formate la chaîne de caractères à retourner
    if (delta < 60) {
      return `${Math.floor(delta)} seconde(s)`;
    } else if (delta < 3600) {
      return `${Math.floor(delta / 60)} minute(s)`;
    } else if (delta < 86400) {
      return `${Math.floor(delta / 3600)} heure(s)`;
    } else {
      return `${Math.floor(delta / 86400)} jour(s)`;
    }
  }
}
