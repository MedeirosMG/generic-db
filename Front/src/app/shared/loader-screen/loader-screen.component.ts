import { Component, OnInit } from '@angular/core';
import { NgxSpinnerService } from 'ngx-spinner';
import { LoaderService } from 'src/app/services/loader.service';

@Component({
  selector: 'app-loader-screen',
  templateUrl: './loader-screen.component.html',
  styleUrls: ['./loader-screen.component.css']
})
export class LoaderScreenComponent implements OnInit {

  constructor(
    private spinner: NgxSpinnerService,
    private loaderService: LoaderService) {
    this.loaderService.isLoading.subscribe((loading) => {

      if (loading)
        this.spinner.show()
      else
        this.spinner.hide()
    });
  }

  ngOnInit(): void {

  }

}
