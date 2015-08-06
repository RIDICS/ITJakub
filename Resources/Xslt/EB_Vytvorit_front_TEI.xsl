<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:xd="http://www.oxygenxml.com/ns/doc/xsl" xmlns:ev="http://www.daliboris.cz/schemata/prepisy.xsd" exclude-result-prefixes="xd ev" version="1.0">
	
	<xd:doc scope="stylesheet">
		<xd:desc>
			<xd:p><xd:b>Created on:</xd:b> Dec 2, 2010</xd:p>
			<xd:p><xd:b>Author:</xd:b> boris</xd:p>
			<xd:p>Vytvoří titulní stránku</xd:p>
		</xd:desc>
	</xd:doc>
	
	<xsl:param name="soubor"/>
	<xsl:output indent="yes"/>
	<xsl:strip-space elements="*"/>
<!--
<xsl:variable name="vroceni" select='2012' />-->
	
	
	
	<xsl:template match="/ev:Prepisy">
		<xsl:apply-templates select="ev:Prepis[ev:Soubor/ev:Nazev = $soubor]"/>
	</xsl:template>
	
	<xsl:template match="ev:Prepis">

		<front>
		<titlePage>
			<figure type="cover" subtype="A4">
				<graphic url="{concat(substring-before($soubor, '.doc'), '.png')}" /></figure>
			<figure type="cover" subtype="A6"><graphic url="{concat(substring-before($soubor, '.doc'), '_A6.png')}" /></figure>
			<docTitle>
				<!--<titlePart><xsl:value-of select="ev:Hlavicka/ev:Titul"/></titlePart>-->
				<titlePart><xsl:value-of select="ev:Zpracovani/ev:Publikace/ev:Vydani[1]/ev:Titul"/></titlePart>
			</docTitle>
			<xsl:apply-templates select="ev:Hlavicka/ev:Autor" />
		</titlePage>

			<div type="editorial" subtype="imprint">
			<xsl:apply-templates select="ev:Hlavicka/ev:Autor" />
			<xsl:apply-templates select="ev:Hlavicka" />
			<xsl:apply-templates select="ev:Hlavicka/ev:EditoriPrepisu" />
			<xsl:call-template name="hlavni-editor" />
			<!--<xsl:call-template name="logo" />-->
			<xsl:call-template name="copyright" >
				<xsl:with-param name="vroceni" select="ev:Zpracovani/ev:Publikace/ev:Vydani[1]/ev:Vroceni/text()" />
			</xsl:call-template>
				<xsl:apply-templates select="ev:Zpracovani/ev:Publikace/ev:Vydani[1]/ev:EvidencniCisla">
				
			</xsl:apply-templates>
			</div>
			<!-- Zajišťuje generování obsahu -->
			<divGen type="toc" />
			</front>
	</xsl:template>

	<xsl:template match="ev:EvidencniCisla">
		<xsl:for-each select="ev:Isbn">
			<p>ISBN <xsl:value-of select="ev:Cislo"/> (<xsl:value-of select="ev:Format"/>)</p>
		</xsl:for-each>
		
	</xsl:template>

<xsl:template name="logo">
	<figure>
		<!--<graphic url="logo.gif"/>-->
		<graphic url="100let.png"/>
	</figure>
</xsl:template>
	
	<xsl:template name="copyright">
		<xsl:param name="vroceni" />
			<p>Copyright ©  <xsl:apply-templates select="ev:Hlavicka/ev:EditoriPrepisu" mode="copyright" /><xsl:text>, </xsl:text><xsl:value-of select="$vroceni"/></p>
			<p>Copyright © oddělení vývoje jazyka Ústavu pro jazyk český AV ČR, v. v. i., <xsl:value-of select="$vroceni"/></p>
			<p>Copyright © Nakladatelství Academia, Středisko společných činností AV ČR, v. v. i., <xsl:value-of select="$vroceni"/></p>
			<p>E-mailový kontakt: vyvoj@ujc.cas.cz</p>
			<p>Obálku navrhl Robin Brichta s použitím obrázku ze Snáře Vavřince z Březové (polovina 15. století, Moravská zemská knihovna v Brně, signatura Mk 0000.0014, fol. 1r). Při tvorbě knihy byla použita písma FreeSerif a FreeSans.</p>
			<!-- TODO: generovat ISBN z evidenčního souboru -->
			<!--<p>ISBN XX-XXXX-XXX-XX (ÚJČ AV ČR, Praha)</p>-->
			<!-- TODO: generovat ISBN z evidenčního souboru -->
			<!--<p>ISBN XX-XXXX-XXX-XX (SSČ AV ČR, Praha)</p>-->
		
	</xsl:template>
	
	<xsl:template name="hlavni-editor">
		<p>Odpovědný redaktor ediční řady: Štěpán Šimek</p>
	</xsl:template>
	
	<xsl:template match="ev:EditoriPrepisu" mode="copyright">
		<xsl:apply-templates select="ev:string" mode="copyright" />
	</xsl:template>
	
	<xsl:template match="ev:string" mode="copyright">
		<xsl:call-template name="jmenoCloveka"/>
		<xsl:if test="not(position() = last())">
			<xsl:text>; </xsl:text>
		</xsl:if>
	</xsl:template>
	
	<xsl:template match="ev:Hlavicka" />
	<xsl:template match="ev:Hlavicka[contains(ev:TypPredlohy, 'Rukopis')]">
			<p>Předloha: <xsl:value-of select="ev:TypPredlohyText"/><xsl:text>, </xsl:text>
				<xsl:value-of select="ev:ZemeUlozeni"/><xsl:text>, </xsl:text>
				<xsl:value-of select="ev:MestoUlozeni"/><xsl:text>, </xsl:text>
				<xsl:value-of select="ev:InstituceUlozeni"/><xsl:text>, sign.: </xsl:text>
				<xsl:value-of select="ev:Signatura"/><xsl:text>, </xsl:text>
				<xsl:value-of select="ev:FoliacePaginace"/>
			</p>
	</xsl:template>
	<xsl:template match="ev:Hlavicka[contains(ev:TypPredlohy, 'StaryTisk')]">
		<p>Předloha: <xsl:text>starý tisk</xsl:text>,
				<xsl:text>vytiskl </xsl:text>
				<xsl:choose>
					<xsl:when test="ev:Tiskar">
						<xsl:value-of select="ev:Tiskar"/>
					</xsl:when>
					<xsl:otherwise>
						<!-- <xsl:text>[neznámý tiskař]</xsl:text> -->
						<!-- požadavek na změnu: Štěpán Šimek, e-mail z 30. 5. 2012 -->
						<xsl:text>neznámý tiskař</xsl:text>						
					</xsl:otherwise>
				</xsl:choose>
				<xsl:text>, </xsl:text>
				<xsl:value-of select="ev:MistoTisku"/><xsl:text> </xsl:text><xsl:value-of select="ev:DataceText"/>
			</p>
	</xsl:template>
	
	<xsl:template match="ev:EditoriPrepisu">
			<p>Elektronickou edici připravili: <xsl:apply-templates select="ev:string" /><xsl:text>Boris Lehečka (kódování TEI)</xsl:text></p>
	</xsl:template>
	
	<xsl:template match="ev:string" />
	
	<xsl:template match="ev:EditoriPrepisu/ev:string">
		<xsl:call-template name="jmenoCloveka"/>
			<xsl:choose>
				<xsl:when test="contains(.,'ová') or ( substring(normalize-space(substring-before(., ',')), string-length(normalize-space(substring-before(., ','))), 1) = 'á')">
					<xsl:text> (editorka), </xsl:text>
				</xsl:when>
				<xsl:otherwise>
					<xsl:text> (editor), </xsl:text>
				</xsl:otherwise>
			</xsl:choose>
		
		
	</xsl:template>
	
	<xsl:template name="jmenoCloveka">
		<xsl:choose>
			<xsl:when test="contains(., ',')">
				<xsl:value-of select="normalize-space(substring-after(., ',')) "/>
				<xsl:text> </xsl:text>
				<xsl:value-of select="normalize-space(substring-before(., ','))"/>
			</xsl:when>
			<xsl:otherwise>
				<xsl:value-of select="."/>
			</xsl:otherwise>
		</xsl:choose>
	</xsl:template>

	<xsl:template match="ev:Autor">
		<docAuthor><xsl:value-of select="."/></docAuthor>
	</xsl:template>
</xsl:stylesheet>