<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" 
	xmlns:xd="http://www.oxygenxml.com/ns/doc/xsl" 
	xmlns:tei="http://www.tei-c.org/ns/1.0"
	xmlns:xml="http://www.w3.org/XML/1998/namespace"
	exclude-result-prefixes="xd tei" version="2.0">
	<xsl:import href="COMMON_Unknown_element.xsl" />
	<xsl:import href="Vokab1550_Prevod-stylu.xsl"/>
	<xsl:import href="Pagina.xsl"/>
	<xd:doc scope="stylesheet">
        <xd:desc>
            <xd:p><xd:b>Created on:</xd:b> Feb 5, 2013</xd:p>
            <xd:p><xd:b>Author:</xd:b> Boris</xd:p>
            <xd:p/>
        </xd:desc>
    </xd:doc>

    <xsl:output method="xml" encoding="UTF-8" indent="yes"/>
    <xsl:strip-space elements="*"/>
	<xsl:preserve-space elements="text"/>
	<xsl:variable name="vychozi-jazyk" select="'cs'"/>

	<xsl:key name="poznamka-pod-carou" match="poznamka_pod_carou" use="@id"/>

<xsl:template match="body">
	<xsl:apply-templates />
</xsl:template>

<xsl:template match="/">
<xsl:text xml:space="preserve">
</xsl:text>
        <xsl:comment> PohlSlov1756_Prevod-Stylu </xsl:comment>
<xsl:text xml:space="preserve">
</xsl:text>
        	<tei:body>
            	<xsl:apply-templates/>
        	</tei:body>
    </xsl:template>


    <xsl:template match="Titul/cestina | Nadpis/cestina"> 
    	<tei:choice>
    		<xsl:apply-templates select="preceding-sibling::transkripce[1]" mode="transcription" />
    		<tei:orig xml:lang="cs-x-translit"><xsl:apply-templates /></tei:orig>
<!--    		<xsl:apply-templates select="following-sibling::transkripce[@jazyk='cestina'][1]" mode="transcription" />-->
    	</tei:choice>
    </xsl:template>
    
    <!-- <xsl:template match="Podnadpis/nemcina | Nadpis/nemcina">
     	<tei:foreign xml:lang="de">
            <xsl:apply-templates/>
        </tei:foreign>
    </xsl:template>-->

    <xsl:template match="Heslova_stat">
    	<tei:entryFree>
            <xsl:apply-templates/>
        </tei:entryFree>
    </xsl:template>


	<xsl:template match="Heslova_stat/transkripce">
		<xsl:variable name="ekvivalent-position">
			<xsl:value-of select="count(following-sibling::nemcina[1]/preceding-sibling::node())"/>
		</xsl:variable>
		<tei:form>
			<tei:orth xml:lang="cs" >
				<tei:choice>
				<tei:reg xml:lang="cs-x-transcr"><xsl:apply-templates select="text()" /></tei:reg>
				<!--<xsl:apply-templates select="following-sibling::*[following-sibling::nemcina]" mode="transliteration" />-->
<!--					<xsl:apply-templates select="following-sibling::node()[position() &lt; $ekvivalent-position]" mode="transliteration" />-->
					<xsl:apply-templates select="following-sibling::nemcina[1]/preceding-sibling::* except self::* except preceding-sibling::*" mode="transliteration" />
				</tei:choice>
			</tei:orth>
		</tei:form>
	</xsl:template>
	
	<xsl:template match="cestina" mode="transliteration">
		<tei:orig xml:lang="cs-x-translit"><xsl:apply-templates /></tei:orig>
	</xsl:template>


	<xsl:template match="cestina" />

<!-- 
	<xsl:template match="Heslova_stat/cestina">
		<xsl:variable name="equivalent">
			<xsl:value-of select="following-sibling::nemcina[1]"/>
		</xsl:variable>
		<xsl:variable name="equivalent-position">
			<xsl:choose>
				<xsl:when test="$equivalent">
					<xsl:value-of select="count($equivalent/preceding-sibling::node()) + 1"/>
				</xsl:when>
			</xsl:choose>
		</xsl:variable>
		<tei:choice>
			<tei:form>
			<tei:orth xml:lang="cs" >
				<xsl:apply-templates select="preceding-sibling::transkripce[1]" mode="transcription" />
				<tei:orig xml:lang="cs-x-translit">
					<xsl:apply-templates select="text()" />
					<xsl:apply-templates select="following-sibling::*[position() &lt; $equivalent-position]" />
				</tei:orig>
				<!-\-					<xsl:apply-templates select="following-sibling::transkripce[@jazyk='cestina'][1]" mode="transcription" /> -\->
			</tei:orth>
		</tei:form>
		</tei:choice>
	</xsl:template>
	-->
	<xsl:template match="transkripce" mode="transcription">
		<tei:reg xml:lang="cs-x-transcr"><xsl:apply-templates /></tei:reg>
	</xsl:template>
	
	<xsl:template match="transkripce" />

	<xsl:template match="*/footnote_reference[not(parent::footnote_text)]">
		<xsl:apply-templates select="key('poznamka-pod-carou', normalize-space(.))" mode="make-inline" />
	</xsl:template>
	
	<xsl:template match="*/footnote_reference[not(parent::footnote_text)]" mode="transliteration">
 		<xsl:apply-templates select="key('poznamka-pod-carou', normalize-space(.))" mode="make-inline" />
 	</xsl:template>
 
	<xsl:template match="poznamka_pod_carou" mode="make-inline">
		<tei:note n="{@id}">
			<xsl:apply-templates/>
		</tei:note>
	</xsl:template>
	
	<xsl:template match="footnote_text">
		<tei:p><xsl:apply-templates/></tei:p>
	</xsl:template>
	
	<xsl:template match="poznamka_pod_carou" />
	
	<xsl:template match="footnote_text/cestina">
<!--			<xsl:element name="tei:cit" namespace="http://www.tei-c.org/ns/1.0">-->
				<xsl:element name="tei:sic" namespace="http://www.tei-c.org/ns/1.0">
					<xsl:attribute name="xml:lang">
						<xsl:text>cs-x-translit</xsl:text>
					</xsl:attribute>
					<xsl:apply-templates/>
				</xsl:element>
			<!--</xsl:element>-->
	</xsl:template>
	
	<xsl:template match="footnote_text/nemcina">
		<!--			<xsl:element name="tei:cit" namespace="http://www.tei-c.org/ns/1.0">-->
		<xsl:element name="tei:sic" namespace="http://www.tei-c.org/ns/1.0">
			<xsl:attribute name="xml:lang">
				<xsl:text>de</xsl:text>
			</xsl:attribute>
			<xsl:apply-templates/>
		</xsl:element>
		<!--</xsl:element>-->
	</xsl:template>
	
	<xsl:template match="footnote_text/text">
<!--		<tei:lbl>-->
			<xsl:apply-templates />
<!--			</tei:lbl>-->
	</xsl:template>
	
	<xsl:template match="delimitator" />
	<xsl:template match="delimitator" mode="transliteration">
		 <!--<tei:lbl type="delim"><xsl:apply-templates /></tei:lbl>-->
	</xsl:template>

	<xsl:template match="footnote_text/footnote_reference" />
	
	
	
	<xsl:template match="cestina_vytceny_vyraz">
		<xsl:element name="supplied">
			<xsl:element name="foreign">
				<xsl:attribute name="xml:lang">
					<xsl:text>cs-x-translit</xsl:text>
				</xsl:attribute>
				<xsl:apply-templates />
			</xsl:element>
		</xsl:element>
	</xsl:template>

	
	<xsl:template match="Pismeno">
		<xsl:element name="head2">
			<xsl:apply-templates/>
		</xsl:element>
	</xsl:template>
	
	
	
	<xsl:template match="Pismeno/text">
		<xsl:element name="foreign">
			<xsl:attribute name="xml:lang">
				<xsl:text>de</xsl:text>
			</xsl:attribute>
			<xsl:apply-templates/>
		</xsl:element>
	</xsl:template>
	
	<xsl:template name="vytvorEkvivalent">
		<xsl:param name="jazyk"/>
		<tei:cit type="translation" xml:lang="{$jazyk}">
			<tei:quote><xsl:apply-templates/></tei:quote>
		</tei:cit>
	</xsl:template>
	
	<!-- delimitátor ekvivalentů (tabulátor) -->
	<xsl:template match="text[. = '&#9;']" />
	
<!--
	<xsl:template match="nemcina_vytceny_vyraz">
		<xsl:element name="supplied">
			<xsl:call-template name="vytvorEkvivalent">
				<xsl:with-param name="jazyk" select="de" />
			</xsl:call-template>
		</xsl:element>
	</xsl:template>
	
	<xsl:template match="doplneny_text">
		<xsl:element name="supplied">
			<xsl:attribute name="xml:lang"><xsl:text>cs-x-translit</xsl:text></xsl:attribute>
			<xsl:apply-templates />
		</xsl:element>
	</xsl:template>
	
	<xsl:template match="doplneny_text_nemcina">
		<xsl:element name="supplied">
			<xsl:attribute name="xml:lang"><xsl:text>de</xsl:text></xsl:attribute>
			<xsl:apply-templates />
		</xsl:element>
	</xsl:template>

	
	<xsl:template match="metajazyk">
		<xsl:element name="gram">
			<xsl:apply-templates />
		</xsl:element>
	</xsl:template>
-->	
</xsl:stylesheet>
