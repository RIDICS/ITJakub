 class SearchBox {
     private inputField: string;
     private options: Twitter.Typeahead.Options;
     private datasets: Twitter.Typeahead.Dataset;

     constructor(inputFieldElement: string, controllerName: string) {
         this.inputField = inputFieldElement;
         var prefetchUrl: string = getBaseUrl() + controllerName + "/GetTypeaheadData";
         var remoteUrl: string = getBaseUrl() + controllerName + "/GetTypeaheadDataForQuery?query=%QUERY";

         var remoteOptions: Bloodhound.RemoteOptions<string> = {
             url: remoteUrl,
             wildcard: "%QUERY"
         };

         var bloodhound = new Bloodhound({
             datumTokenizer: Bloodhound.tokenizers.whitespace,
             queryTokenizer: Bloodhound.tokenizers.whitespace,
             prefetch: prefetchUrl,
             remote: remoteOptions
         });


         this.options = {
             hint: true,
             highlight: true,
             minLength: 2
         };
         this.datasets = {
             name: controllerName,
             source: bloodhound
         };
     }

     create(): void {
         $(this.inputField).typeahead(this.options, this.datasets);
     }

     destroy(): void {
         $(this.inputField).typeahead("destroy");
     }
 }