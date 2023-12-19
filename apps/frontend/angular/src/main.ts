import { platformBrowserDynamic } from '@angular/platform-browser-dynamic';

import { AppModule } from './app/app.module';

import { Amplify } from 'aws-amplify';
import aws_exports from './aws-exports';

// import awsconfig from "./aws-exports";
// import { environment } from './environments/environment';

// var redirectSignIn: Array<String> = awsconfig["oauth"].redirectSignIn.split(',')
// var redirectSignOut: Array<String> = awsconfig["oauth"].redirectSignOut.split(',');

// // store staging urls locally
// var stagingSignIn = redirectSignIn.filter(x=> x.includes('staging'))[0];
// var stagingSignOut = redirectSignOut.filter(x=> x.includes('staging'))[0];

// // filter out the staging urls
// redirectSignIn = redirectSignIn.filter(x=> x != stagingSignIn);
// redirectSignOut = redirectSignOut.filter(x=> x != stagingSignOut);

// // store production urls locally
// var productionSignIn = redirectSignIn.filter(x=> !x.includes('mobile'));
// var productionSignOut = redirectSignOut.filter(x=> !x.includes('mobile'));

// if (environment.branch == "main") {
//   awsconfig["oauth"].redirectSignIn = productionSignIn;
//   awsconfig["oauth"].redirectSignOut = productionSignOut;
// } else {
//   awsconfig["oauth"].redirectSignIn = stagingSignIn;
//   awsconfig["oauth"].redirectSignOut = stagingSignOut;
// }

Amplify.configure(aws_exports);

platformBrowserDynamic().bootstrapModule(AppModule)
  .catch(err => console.error(err));