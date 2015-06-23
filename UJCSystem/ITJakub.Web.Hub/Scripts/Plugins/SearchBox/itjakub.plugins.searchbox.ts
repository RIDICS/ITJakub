 class SearchBox {
     private inputField: string;
     private controllerName: string;
     private urlWithController: string;
     private options: Twitter.Typeahead.Options;
     private datasets: Array<Twitter.Typeahead.Dataset>;

     constructor(inputFieldElement: string, controllerName: string) {
         this.inputField = inputFieldElement;
         this.controllerName = controllerName;
         this.urlWithController = getBaseUrl() + controllerName;
         this.datasets = [];

         this.options = {
             hint: true,
             highlight: true,
             minLength: 1
         };
     }

     create(): void {
         $(this.inputField).typeahead(this.options, this.datasets);
     }

     destroy(): void {
         $(this.inputField).typeahead("destroy");
     }

     addDataSet(name: string, groupHeader: string): void {
         var prefetchUrl: string = this.urlWithController + "/GetTypeahead" + name;
         var remoteUrl: string = this.urlWithController + "/GetTypeahead" + name + "?query=%QUERY";

         var remoteOptions: Bloodhound.RemoteOptions<string> = {
             url: remoteUrl,
             wildcard: "%QUERY"
         };

         var bloodhound: Bloodhound<string> = new Bloodhound({
             datumTokenizer: Bloodhound.tokenizers.whitespace,
             queryTokenizer: Bloodhound.tokenizers.whitespace,
             prefetch: prefetchUrl,
             remote: remoteOptions
         });

         var dataset: Twitter.Typeahead.Dataset = {
             name: this.controllerName,
             limit: 5,
             source: bloodhound,
             templates: {
                 header: "<div class=\"tt-suggestions-header\">" + groupHeader + "</div>"
             }
         };

         this.datasets.push(dataset);
     }
 }