import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import {MatProgressSpinnerModule} from '@angular/material/progress-spinner';
import {MatBadgeModule} from '@angular/material/badge';
import {MatIconModule} from '@angular/material/icon';

@NgModule({
  declarations: [],
  imports: [
    CommonModule,
    MatProgressSpinnerModule,
    MatBadgeModule,
    MatIconModule
  ],
  exports:[
    MatProgressSpinnerModule,
    MatBadgeModule,
    MatIconModule
  ]
})
export class MaterialAngularModule { }
