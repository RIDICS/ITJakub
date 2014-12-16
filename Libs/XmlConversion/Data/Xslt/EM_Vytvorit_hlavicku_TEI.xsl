<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:xd="http://www.oxygenxml.com/ns/doc/xsl" xmlns:ev="http://www.daliboris.cz/schemata/prepisy.xsd" exclude-result-prefixes="xd ev" version="1.0">
	<xd:doc scope="stylesheet">
		<xd:desc>
			<xd:p><xd:b>Created on:</xd:b> Dec 2, 2010</xd:p>
			<xd:p><xd:b>Author:</xd:b> boris</xd:p>
			<xd:p>Vytvoří hlavičku TEI na základě dat z evidenčního XML textů</xd:p>
		</xd:desc>
	</xd:doc>

	<xsl:param name="soubor"/>
	<xsl:output indent="yes"/>
	<xsl:strip-space elements="*"/>



	<xsl:template match="/ev:Prepisy">
		<xsl:apply-templates select="ev:Prepis[ev:Soubor/ev:Nazev = $soubor]"/>
	</xsl:template>

	<xsl:template match="ev:Prepis">
		<teiHeader>
			<fileDesc>
				<xsl:attribute name="n">
					<xsl:value-of select="ev:Zpracovani/ev:GUID"/>
				</xsl:attribute>
				<titleStmt>
					<title>
						<xsl:value-of select="ev:Hlavicka/ev:Titul"/>
					</title>
					<xsl:apply-templates select="ev:Hlavicka/ev:Autor" mode="titleStmt"/>
				</titleStmt>


				<editionStmt>
					<edition>elektronická edice</edition>
					<xsl:apply-templates select="ev:Hlavicka/ev:EditoriPrepisu" />
					<respStmt>
						<resp>kódování TEI</resp>
						<name>Lehečka, Boris</name>
					</respStmt>
				</editionStmt>
				
				
				<publicationStmt>
					<publisher>oddělení vývoje jazyka Ústavu pro jazyk český AV ČR, v. v. i.<email>vyvoj@ujc.cas.cz</email>
					</publisher>
					<pubPlace>Praha</pubPlace>
					<!-- TODO: načítat datum publikace programově -->
                    <date><xsl:value-of select="'2012'"/></date>
					<!--<publisher>Manuscriptorium.com</publisher>-->
					<availability status="restricted">
						<p>Tato edice je autorské dílo chráněné ve smyslu zákona č. 121/2000 Sb., o právu autorském, a je určena pouze k nekomerčním účelům.</p>
					</availability>
				</publicationStmt>
				
				
				<sourceDesc>
					<msDesc xml:lang="cs">
						<xsl:call-template name="identifikaceRukopisu" />
						<msContents>
							<msItem>
								<title><xsl:value-of select="ev:Hlavicka/ev:Titul"/></title>
							</msItem>
						</msContents>
						<history>
							<!--
								
								xw.WriteStartElement("origDate");
								if (dt.Rok != 0 && (dt.NePoRoce == dt.NePredRokem) ) {
								xw.WriteAttributeString("when", dt.Rok.ToString());
								}
								else {
								if (!glsUpresneni.Contains(dt.Upresneni)) {
								xw.WriteAttributeString("notBefore", dt.NePredRokem.ToString());
								xw.WriteAttributeString("notAfter", dt.NePoRoce.ToString());
								}
								}
								xw.WriteString(dt.SlovniPopis);
								xw.WriteEndElement();//</origDate>
							-->
							
							<origin>
								<origDate notBefore="{ev:Hlava/ev:Pamatka/ev:Pramen/ev:Datace/ev:NePredRokem}" notAfter="{ev:Hlava/ev:Pamatka/ev:Pramen/ev:Datace/ev:NePoRoce}"><xsl:value-of select="ev:Hlavicka/ev:DataceText"/></origDate>
							</origin>
						</history>
					</msDesc>
				</sourceDesc>
			</fileDesc>
			<xsl:call-template name="encodingDesc" />
			<!--<revisionDesc>
				<change when="{substring-before(ev:Zpracovani/ev:Exporty/ev:Export[ev:ZpusobVyuziti = 'Manuscriptorium'][last()]/ev:CasExportu, 'T')}"/>
			</revisionDesc>-->
		</teiHeader>
	</xsl:template>

	<xsl:template name="encodingDesc">
		<encodingDesc>
			<classDecl>
				<taxonomy xml:id="taxonomy">
					<category xml:id="taxonomy-dictionary">
						<catDesc xml:lang="cs-cz">slovník</catDesc>
						<category xml:id="taxonomy-dictionary-contemporary">
							<catDesc xml:lang="cs-cz">soudobý</catDesc>
						</category>
						<category xml:id="taxonomy-dictionary-historical">
							<catDesc xml:lang="cs-cz">dobový</catDesc>
						</category>
					</category>
					<category xml:id="taxonomy-historical_text">
						<catDesc xml:lang="cs-cz">historický text</catDesc>
						<category xml:id="taxonomy-historical_text-old_czech">
							<catDesc xml:lang="cs-cz">staročeský</catDesc>
						</category>
						<category xml:id="taxonomy-historical_text-medieval_czech">
							<catDesc xml:lang="cs-cz">středněčeský</catDesc>
						</category>
					</category>
					<category xml:id="taxonomy-scholary_text">
						<catDesc xml:lang="cs-cz">odborný text</catDesc>
					</category>
					<category xml:id="taxonomy-digitized-grammar">
						<catDesc xml:lang="cs-cz">digitalizovaná mluvnice</catDesc>
					</category>
					<category xml:id="taxonomy-card-index">
						<catDesc xml:lang="cs-cz">lístková kartotéka</catDesc>
					</category>
				</taxonomy>
			</classDecl>
		</encodingDesc>
		<profileDesc>
			<textClass>
				<xsl:call-template name="catRef" />
				<keywords scheme="http://vokabular.ujc.cas.cz/scheme/classification/secondary">
					<xsl:apply-templates select="ev:Zpracovani/ev:LiterarniDruh" mode="term" />
					<xsl:apply-templates select="ev:Zpracovani/ev:LiterarniZanr" mode="term" />
				</keywords>
			</textClass>
		</profileDesc>

	</xsl:template>

	<xsl:template name="catRef">
		<xsl:element name="catRef">
			<xsl:attribute name="target">
				<xsl:choose>
					<xsl:when test="ev:TypPrepisu/. = 'edice' and ev:Zpracovani/ev:CasoveZarazeni/. = 'DoRoku1500'">
						<xsl:text>#taxonomy-historical_text-old_czech</xsl:text>
					</xsl:when>
					<xsl:when test="ev:TypPrepisu/. = 'edice' and ev:Zpracovani/ev:CasoveZarazeni/. = 'DoRoku1800'">
						<xsl:text>#taxonomy-historical_text-medieval_czech</xsl:text>
					</xsl:when>
					<xsl:when test="ev:TypPrepisu/. = 'odborná literatura'">
						<xsl:text>#taxonomy-scholary_text</xsl:text>
					</xsl:when>
				</xsl:choose>

			</xsl:attribute>
		</xsl:element>

	</xsl:template>

	<xsl:template match="ev:LiterarniDruh | ev:LiterarniZanr" mode="term">
		<term>
			<xsl:apply-templates />
		</term>
	</xsl:template>


	<xsl:template name="identifikaceRukopisu">
		<msIdentifier>
				<xsl:apply-templates select="ev:Hlavicka/ev:ZemeUlozeni" />
				<xsl:apply-templates select="ev:Hlavicka/ev:MestoUlozeni" />
				<xsl:apply-templates select="ev:Hlavicka/ev:InstituceUlozeni" />
				<xsl:apply-templates select="ev:Hlavicka/ev:Signatura" />
		</msIdentifier>
	</xsl:template>

<xsl:template match="ev:Signatura">
	<idno><xsl:value-of select="."/></idno>	
</xsl:template>

	<xsl:template match="ev:InstituceUlozeni">
		<repository><xsl:value-of select="."/></repository>
	</xsl:template>
	
	
	<xsl:template match="ev:MestoUlozeni">
		<settlement><xsl:value-of select="."/></settlement>
	</xsl:template>
	
	<xsl:template match="ev:ZemeUlozeni">
		<country>
			<xsl:attribute name="key">
		<xsl:choose>
			<xsl:when test=". = 'Česko'">xr</xsl:when>
			<xsl:when test=". = 'Rakousko'">au</xsl:when>
			<xsl:when test=". = 'Polsko'">pl</xsl:when>
			<xsl:when test=". = 'Francie'">fr</xsl:when>
			<xsl:when test=". = 'Německo'">gw</xsl:when>
			<xsl:when test=". = 'Rusko'">ru</xsl:when>
			<xsl:when test=". = 'Slovensko'">xo</xsl:when>
			<xsl:when test=". = 'Maďarsko'">hu</xsl:when>
			<xsl:when test=". = 'Rakousko'">au</xsl:when>
		</xsl:choose>
			</xsl:attribute>
			<xsl:value-of select="."/>
		</country>
	</xsl:template>

	<xsl:template match="ev:EditoriPrepisu">
			
		<xsl:apply-templates />
<!--		<xsl:for-each select="ev:string">
			<xsl:apply-templates select="." />
		</xsl:for-each>-->
	</xsl:template>

<xsl:template match="ev:EditoriPrepisu/ev:string">
	<respStmt>
		<resp>editor</resp>
	<name>
		<xsl:value-of select="."/>
	</name>
	</respStmt>
</xsl:template>
	
	<xsl:template match="ev:Autor" mode="titleStmt">
		<xsl:element name="author">
			<xsl:value-of select="."/>
		</xsl:element>
	</xsl:template>

</xsl:stylesheet>
