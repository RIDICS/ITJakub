<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
	xmlns:xd="http://www.oxygenxml.com/ns/doc/xsl"
	xmlns="http://www.tei-c.org/ns/1.0"
	exclude-result-prefixes="xd"
	version="1.0">
	<xd:doc scope="stylesheet">
		<xd:desc>
			<xd:p><xd:b>Created on:</xd:b> Dec 4, 2010</xd:p>
			<xd:p><xd:b>Author:</xd:b> boris</xd:p>
			<xd:p></xd:p>
		</xd:desc>
	</xd:doc>
	
	<xsl:include href="EB_Souhrnna_edicni_poznamka.xsl" />
	
	<xsl:param name="hlavicka" />
	<xsl:param name="zacatek" />
	<xsl:output omit-xml-declaration="no" indent="yes"/>
	<xsl:strip-space elements="*"/>
	
	<xsl:template match="/">
		<xsl:comment> EB_Vybor_Spojit_s_hlavickou_a_front </xsl:comment>
		<TEI xmlns="http://www.tei-c.org/ns/1.0" xml:lang="cs">
			<!-- 1: vydání -->
			<!--<xsl:call-template name="hlavicka" />-->
			<xsl:apply-templates select="document($hlavicka)/*" mode="jmennyProstor" />
			<text>
				<xsl:apply-templates select="document($zacatek)/*" mode="jmennyProstor" />
					<xsl:apply-templates mode="jmennyProstor" />
				<xsl:call-template name="konec" />
			</text>
			<!-- Doplněno 18. 11. 2010 -->
			</TEI>
	</xsl:template>

	<xsl:template name="konec">
		<back>
		<!--	<xsl:call-template name="edicniPoznamka" />-->
			<div type="editorial" subtype="annotation">
				<p><hi rend="it">Výbor ze starší české literatury </hi>obsahuje soubor šestadvaceti česky psaných textů nebo jejich úryvků z období od 13. do 18. století. Středověkou literaturu reprezentují např. díla veršované lyriky, erbovní pověst o Bruncvíkovi, úryvky z biblické knihy Genesis a z děl Tomáše ze Štítného i Jana Husa a ukázky z odborné středověké literatury lékařské a právnické. Raný novověk je zastoupen mj. spisy zábavního charakteru – cestopisem, satirami, kuchařkou atd.</p> 
					<p><hi rend="it">Výbor ze starší české literatury </hi>stojí na počátku nového edičního projektu a je „ochutnávkou“ z chystaných knih. Edičně jej připravilo <hi rend="it">oddělení vývoje jazyka Ústavu pro jazyk český AV ČR, v. v. i.</hi> Vydávané texty jsou transkribovány do novočeského pravopisného systému a obsahují nezbytný textověkritický komentář.</p>
			</div>
			<!-- Anotace, která se objeví jako popiska/anotace elektronické knihy v e-shopu -->
<!--			<div type="editorial" subtype="annotation">
				<p>Anotace elektronické edice.</p>
			</div>
-->			<!-- Upozornění na autorská práva, které se objeví při stahování elektronické knihy  -->
			<div type="editorial" subtype="copyright">
				<p>Tato edice je autorské dílo chráněné ve smyslu zákona č. 121/2000 Sb., o právu autorském, a je určena pouze k nekomerčním účelům.</p>
			</div>
		</back>
	</xsl:template>
	
	<xsl:template match="*" mode="jmennyProstor">
		<xsl:element name="{local-name()}" namespace="http://www.tei-c.org/ns/1.0">
			<xsl:apply-templates select="@*|*|text()" mode="jmennyProstor"/>
		</xsl:element>
	</xsl:template>
	
	<xsl:template match="@*" mode="jmennyProstor">
		<xsl:variable name="nazev">
		<xsl:choose>
			<xsl:when test="local-name() = 'id'"><xsl:text>xml:id</xsl:text></xsl:when>
			<xsl:when test="local-name() = 'lang'"><xsl:text>xml:lang</xsl:text></xsl:when>
			<xsl:otherwise><xsl:value-of select="local-name()"/></xsl:otherwise>
		</xsl:choose>
		</xsl:variable>
		<xsl:attribute name="{$nazev}">
			<xsl:value-of select="."/>
		</xsl:attribute>
	</xsl:template>
	
	
	<xsl:template name="hlavicka">
		<teiHeader>
			<fileDesc n="2A100BE0-D058-486C-8E27-63801CDFDA22">
				<titleStmt>
					<title>Výbor ze starší české literatury</title>
					<author>oddělení vývoje jazyka Ústavu pro jazyk český AV ČR, v. v. i.</author>
				</titleStmt>
				<editionStmt>
					<edition>elektronická edice</edition>
					<respStmt><resp>hlavní editorka</resp><name>Černá, Alena M.</name></respStmt>
					<respStmt><resp>editor</resp><name>Boček, Miroslav</name></respStmt>
					<respStmt><resp>editor</resp><name>Fuková, Irena</name></respStmt>
					<respStmt><resp>editor</resp><name>Hanzová, Barbora</name></respStmt>
					<respStmt><resp>editor</resp><name>Jamborová, Martina</name></respStmt>
					<respStmt><resp>editor</resp><name>Janosik-Bielski, Marek</name></respStmt>
					<respStmt><resp>editor</resp><name>Pečírková, Jaroslava</name></respStmt>
					<respStmt><resp>editor</resp><name>Šimek, Štěpán</name></respStmt>
					<respStmt><resp>editor</resp><name>Voleková, Kateřina</name></respStmt>
					<respStmt><resp>editor</resp><name>Zápotocká, Pavlína</name></respStmt>
					<respStmt><resp>kódování TEI</resp><name>Lehečka, Boris</name></respStmt>
				</editionStmt>
				<publicationStmt>
					<publisher>oddělení vývoje jazyka Ústavu pro jazyk český AV ČR, v. v. i.</publisher>
					<pubPlace>Praha</pubPlace>
					<date><xsl:call-template name="vroceni"/></date>
					<availability status="restricted">
						<p>Tato edice je autorské dílo chráněné ve smyslu zákona č. 121/2000 Sb., o právu autorském, a je určena pouze k nekomerčním účelům.</p>
					</availability>
					<!--<idno type="ISBN_E">978-80-86496-50-4</idno>-->
					<idno type="ISBN_PDF"><xsl:call-template name="IsbnPdf"/></idno>
					<idno type="ISBN_EPUB"><xsl:call-template name="IsbnEpub"/></idno>
					<idno type="ISBN_P"/>
				</publicationStmt>
				<sourceDesc>
					<msDesc xml:lang="cs">
							<msIdentifier>
								<idno type="PDF"><xsl:call-template name="IsbnPdf"/></idno>
						<altIdentifier>
							<idno type="EPUB"><xsl:call-template name="IsbnEpub"/></idno>
						</altIdentifier>
					</msIdentifier>
						<msContents>
							<msItem>
								<title>Výbor ze starší české literatury</title>
							</msItem>
						</msContents>
						<history>
							<origin><origDate notBefore="1200" notAfter="1750">13.–18. století</origDate></origin>
						</history>
					</msDesc>
				</sourceDesc>
			</fileDesc>
					<profileDesc>
			<textClass>
				<keywords scheme="http://vkabular.ujc.cas.cz/">
					<term>starší česká literatura</term>
					<term>próza</term>
					<term>poezie</term>
					<term>výbor</term>
				</keywords>
			</textClass>
		</profileDesc>
		</teiHeader>
	</xsl:template>
	<xsl:template name="IsbnEpub"> 
		<!-- 1. vydání -->
		<!--<xsl:value-of select="'978-80-86496-51-1'"/>-->
		<!-- 2. vydání -->
		<xsl:value-of select="'978-80-86496-XX-X'"/>
	</xsl:template>
	<xsl:template name="IsbnPdf">
		<!-- 1. vydání -->
		<!--<xsl:value-of select="'978-80-86496-50-4'"/>-->
		<!-- 2. vydání -->
		<xsl:value-of select="'978-80-86496-XX-X'"/>
	</xsl:template>
	<xsl:template name="vroceni">
		<!-- 1. vydání -->
		<!--<xsl:value-of select="'2011'"/>-->
		<!-- 2. vydání -->
		<xsl:value-of select="'2013'"/>
	</xsl:template>
</xsl:stylesheet>