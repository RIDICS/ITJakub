1. Move *.container.config to root folder of deployed service
2. Install eXistDB version 2.1.
3. Copy content of "ExistDB" folder except "config" folder to app collection named "jacob".
4. Copy content of "ExistDB/config" folder to collection "/system/config/db/apps/jacob".
5. Install Altova XML 2013 community edition
6. Deploy Each service into different App pool in IIS  >8 
7. Exist DB - Http 413 "entity too large"
	7.1 Might not be needed because we use POST for larger queries.
    7.2 Workaround: 
	copy <Set name="requestHeaderSize">sizeInBytes</Set> to jetty.xml (i.e. C:\eXist-db\tools\jetty\etc\jetty.xml) under <New class="org.eclipse.jetty.server.nio.SelectChannelConnector"> element.
	Example:
	
  <!-- ============================================================ -->
  <!-- Set connectors                                               -->
  <!-- See http://wiki.eclipse.org/Jetty/Howto/Configure_Connectors -->
  <!-- ============================================================ -->

  <Call name="addConnector">
    <Arg>
      <New class="org.eclipse.jetty.server.nio.SelectChannelConnector">
        <Set name="host"><SystemProperty name="jetty.host"/></Set>
        <Set name="port"><SystemProperty name="jetty.port" default="8080"/></Set>
        <Set name="maxIdleTime">300000</Set>				
		<Set name="requestHeaderSize">409600</Set>
        <Set name="Acceptors">2</Set>
        <Set name="statsOn">false</Set>
        <Set name="confidentialPort"><SystemProperty name="jetty.port.ssl" default="8443"/></Set>
        <Set name="lowResourcesConnections">20000</Set>
        <Set name="lowResourcesMaxIdleTime">5000</Set>
      </New>
    </Arg>
  </Call>
8. Install certificates
	 8.1. - Install ITJakubCA to trusted root certificate authrorities for local computer
	 8.2. - Install certificates for ITJakubClient and ITJakubService to Personal store in Local computer
	 8.3. - Click on each certificate in personal store and select "Manage private keys" and add "Everyone" for full control to all certificates
	 8.4. - in IIS manager select website and add binding with localhost certificate issued by ITJakubCA
9. Allow SSL in IIS - Edit bindings for Default Web Site and Add HTTPS with localhost (ITJAKUBCA) certificate. Allow SSL for default Website and select "Accept client certificate" in SSL settings for default WebSite