1. Move *.container.config to root folder of deployed service
2. Xquery GetDocument in existDB folder modules change resource path to actual project name in existDB
3. Install Altova XML 2013 community edition
4. Deploy Each service into different App pool in IIS  >8 
5. Exist DB - Http 413 "entity too large" error workaround: 
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