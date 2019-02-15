import { Component, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';


@Component({
  selector: 'business-flows-data',
  templateUrl: './businessflows.component.html'
})


export class BusinessFlowsComponent
{
  public businessflows: BusinessFlow[];
  public report: string;
  mHttp: HttpClient;
  mBaseUrl: string;

  
  constructor(http: HttpClient, @Inject('BASE_URL') baseUrl: string)
  {
    this.mHttp = http;
    this.mBaseUrl = baseUrl;

    http.get<BusinessFlow[]>(baseUrl + 'api/BusinessFlow/BusinessFlows').subscribe(result => {
      this.businessflows = result;
    }, error => console.error(error));

  }

  public runFlow(BF:BusinessFlow) {
    BF.status = "Running";
    BF.elapsed = -1;
    const req = this.mHttp.post<RunBusinessFlowResult>(this.mBaseUrl + 'api/BusinessFlow/RunBusinessFlow', {
      name: BF.name  //TODO: We send the BF name replace with BF.Guid
    })
      .subscribe(
      res => {
        // Once we get the response        
        BF.status = res.status;
        BF.elapsed = res.elapsed;
        // this.report = res.report;
      },
        err => {
          console.log("Error occured");
          BF.status = "Exception while run flow";
        }
      );
  }

  public flowReport(BF: BusinessFlow) {
    
  }



}



interface RunBusinessFlowResult {
  status: string;
  elapsed: number;
  report: string;
}

interface BusinessFlow {
  name: string;
  description: string;
  fileName: string;
  status: string;
  elapsed: number;  
}
