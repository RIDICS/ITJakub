 class SearchBox {
     private inputField: string;
     private urlWithController: string;
     private options: Twitter.Typeahead.Options;
     private datasets: Array<Twitter.Typeahead.Dataset>;
     private bloodhounds: Array<Bloodhound<string>>;

     constructor(inputFieldElement: string, controllerPath: string) {
         this.inputField = inputFieldElement;
         this.urlWithController = getBaseUrl() + controllerPath;
         this.datasets = [];
         this.bloodhounds = [];

         this.options = {
             hint: true,
             highlight: true,
             minLength: 1
         };
     }

     value(value: any):void {
         $(this.inputField).typeahead('val', value);
     }

     create(): void {
         $(this.inputField).typeahead(this.options, this.datasets);
     }

     destroy(): void {
         $(this.inputField).typeahead("destroy");
     }

     clearAndDestroy(): void {
         for (var i = 0; i < this.bloodhounds.length; i++) {
             var bloodhound = this.bloodhounds[i];
             bloodhound.clear();
             bloodhound.clearPrefetchCache();
             bloodhound.clearRemoteCache();
         }
         this.datasets = [];
         this.bloodhounds = [];
         this.destroy();
     }

     addDataSet(name: string, groupHeader: string, parameterUrlString: string = null): void {
         var prefetchUrl: string = this.urlWithController + "/GetTypeahead" + name;
         var remoteUrl: string = this.urlWithController + "/GetTypeahead" + name + "?query=%QUERY";

         if (parameterUrlString != null) {
             prefetchUrl += "?" + parameterUrlString;
             remoteUrl += "&" + parameterUrlString;
         }

         var remoteOptions: Bloodhound.RemoteOptions<string> = {
             url: remoteUrl,
             wildcard: "%QUERY"
         };

         var bloodhound: Bloodhound<string> = new Bloodhound({
             datumTokenizer: Bloodhound.tokenizers.whitespace,
             queryTokenizer: Bloodhound.tokenizers.whitespace,
             prefetch: prefetchUrl,
             remote: remoteOptions,
             limit: 5
         });


         var dataset: Twitter.Typeahead.Dataset = {
             name: name,             
             source: bloodhound,
             templates: {
                 header: "<div class=\"tt-suggestions-header\">" + groupHeader + "</div>"
             }
         };

         this.bloodhounds.push(bloodhound);
         this.datasets.push(dataset);
     }
 }