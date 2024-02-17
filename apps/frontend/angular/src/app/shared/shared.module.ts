import { NgModule } from '@angular/core';
import { LoadingComponent } from './loading/loading.component';
import { HttpClientModule } from '@angular/common/http';
import { ReactiveFormsModule } from '@angular/forms';
import { CarouselComponent } from './carousel/carousel.component';
import { NavigationBarComponent } from './navigation-bar/navigation-bar.component';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { DartboardComponent } from './dartboard/dartboard.component';

@NgModule({
  declarations: [
    CarouselComponent,
    NavigationBarComponent,
    DartboardComponent
  ],
  imports: [
    CommonModule,
    HttpClientModule,
    ReactiveFormsModule,
    RouterModule
  ],
  exports: [
    CarouselComponent,
    NavigationBarComponent,
    DartboardComponent
  ]
})

export class SharedModule { }
