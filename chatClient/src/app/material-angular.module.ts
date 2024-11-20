import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import {MatProgressSpinnerModule} from '@angular/material/progress-spinner';
import {MatBadgeModule} from '@angular/material/badge';
import {MatIconModule} from '@angular/material/icon';
import {MatInputModule} from '@angular/material/input';
import {MatAutocompleteModule} from '@angular/material/autocomplete';
@NgModule({
  declarations: [],
  imports: [
    CommonModule,
    MatProgressSpinnerModule,
    MatBadgeModule,
    MatIconModule,
    MatInputModule,
    MatAutocompleteModule
  ],
  exports:[
    MatProgressSpinnerModule,
    MatBadgeModule,
    MatIconModule,
    MatInputModule,
    MatAutocompleteModule
  ]
})
export class MaterialAngularModule { }
