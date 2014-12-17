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
	<xsl:variable name="vroceni" select="'2011'" />
	<xsl:variable name="IsbnPdf" select="'978-80-86496-50-4'" />
	<xsl:variable name="IsbnEpub" select="'978-80-86496-51-1'" />
	-->
	
	<xsl:variable name="vroceni" select="'2013'" />
	<xsl:variable name="IsbnPdf" select="'978-80-86496-73-3'" />
	<xsl:variable name="IsbnEpub" select="' 978-80-86496-74-0'" />
	<xsl:variable name="obalka" select="'Vybor_ze_starsi_ceske_literatury_2_vydani'" /> <!-- původně 'Obalka' -->
	
	<xsl:template match="/ev:Prepisy">
		<xsl:comment> EB_Vybor_Vytvorit_front_TEI </xsl:comment>
		<xsl:apply-templates select="ev:Prepis[ev:Soubor/ev:Nazev = $soubor]"/>
	</xsl:template>

	<xsl:template name="podtitul">
		<titlePart type="sub">druhé, doplněné vydání</titlePart>
	</xsl:template>
	
	<xsl:template match="ev:Prepis">
		<front>
			<titlePage>
								<figure type="cover" subtype="A4"><graphic url="{concat($obalka, '.png')}" /></figure>
				<figure type="cover" subtype="A6"><graphic url="{concat($obalka, '_A6.png')}" /></figure>
				<docTitle>
					<titlePart>
						<xsl:value-of select="ev:Hlavicka/ev:Titul"/>
					</titlePart>
					<xsl:call-template name="podtitul"/>
				</docTitle>
				<xsl:apply-templates select="ev:Hlavicka/ev:Autor"/>
			</titlePage>
			<!-- 1. vydání -->
			<!--<xsl:call-template name="venovani"/>-->
			<!-- 2. vydání -->
			<!--<xsl:call-template name="venovani"/>-->
			<!--<xsl:call-template name="venovani_eng"/>-->
			<div>
				<xsl:apply-templates select="ev:Hlavicka"/>
				<xsl:apply-templates select="ev:Hlavicka/ev:EditoriPrepisu"/>
				<!-- 1. vydání -->
<!--				<xsl:call-template name="logo"/>
				<xsl:call-template name="vyroci"/>
				<xsl:call-template name="copyright"/>
				<xsl:call-template name="kontakt" />
				<xsl:call-template name="upozorneni"/>
				<xsl:call-template name="vydal"/>
-->				<!-- 2. vydání -->
				<xsl:call-template name="copyright">
<!--					<xsl:with-param name="vroceni" select="'2011'" />-->
					<xsl:with-param name="vroceni" select="ev:Zpracovani/ev:Publikace/ev:Vydani[1]/ev:Vroceni" />
				</xsl:call-template>
				<xsl:call-template name="kontakt" />
				<xsl:call-template name="upozorneni"/>
				<xsl:call-template name="vydal"/>
			</div>
			<divGen type="toc"/>
		</front>
	</xsl:template>

<xsl:template name="kontakt">
		<div type="editorial">
			<p>E-mailový kontakt: vyvoj@ujc.cas.cz</p>
		</div>
	</xsl:template>
	<xsl:template name="upozorneni">
		<div type="editorial">
			<p>Obálku navrhl Robin Brichta s použitím obrázku ze Snáře Vavřince z Březové (polovina 15. století, Moravská zemská knihovna v Brně, signatura Mk 0000.0014, fol. 1r). Při tvorbě knihy bylo použito písmo FreeSerif.</p>
		</div>
		<div type="editorial">
			<div type="editorial">
				<p>Kniha vznikla v rámci řešení projektu GA ČR č. P406/10/1140 <hi rend="it">Výzkum historické češtiny (na základě nových materiálových bází)</hi>.</p>
			</div>
		</div>
	</xsl:template>

	<xsl:template name="vydal">
		<div type="editorial">
<!--			<p>ISBN <xsl:value-of select="$IsbnPdf"/> (PDF)</p>
			<p>ISBN <xsl:value-of select="$IsbnEpub"/> (EPUB)</p>
-->
			<p>ISBN <xsl:value-of select="ev:Zpracovani/ev:Publikace/ev:Vydani[1]/ev:EvidencniCisla/ev:Isbn[ev:Format/text() = 'EPUB']/ev:Cislo"/> (PDF)</p>
			<p>ISBN <xsl:value-of select="ev:Zpracovani/ev:Publikace/ev:Vydani[1]/ev:EvidencniCisla/ev:Isbn[ev:Format/text() = 'PDF']/ev:Cislo"/> (EPUB)</p>
		</div>
		<div type="editorial" subtype="loga">
					<figure subtype="A4"><graphic url="Logo_ujc.png"/></figure>
					<figure subtype="A6"><graphic url="Logo_ujc_A6.png"/></figure>
					<figure subtype="A4"><graphic url="Logo_academia.png"/></figure>
					<figure subtype="A6"><graphic url="Logo_academia_A6.png"/></figure>
		</div>
		<div type="editorial">
			<p>Vydal Ústav pro jazyk český AV ČR, v. v. i., v Nakladatelství Academia.</p>
			<p>Praha <xsl:value-of select="$vroceni"/>.</p>
		</div>
	</xsl:template>

	<xsl:template name="logo">
						<figure subtype="A4"><graphic url="100let.png" /></figure>
				<figure subtype="A6"><graphic url="100let_A6.png" /></figure>
	</xsl:template>

	<xsl:template name="vyroci">
		<!-- Tmavě modře, drobnějším písmem, spojit s obrázkem, asi vystředit -->
		<div type="vyroci">
			<p>Tato kniha vychází v roce stého výročí Ústavu pro jazyk český.</p>
		</div>
	</xsl:template>

	<xsl:template name="venovani_eng">
				<div type="venovani">
					<p rend="nazev">Anthology of Early Czech Literature</p>
					<p rend="nazev2">Institute of the Czech Language of the Academy of Sciences of the Czech Republic, Department of Language Development</p>
					<p rend="vychazi">This book is published on the 100<hi rend="sup">th</hi> anniversary of the Institute of the Czech Language</p>
							<figure subtype="A4"><graphic url="100let_venovani_A4.png" /></figure>
				<figure subtype="A6"><graphic url="100let_venovani_A6.png" /></figure>

					<p rend="komu">This copy is Dedicated to the Honorary Consul General of the Czech Republic Richard Pivnicka</p>
			<table rend="podpisy">
				<row>
					<cell>
						<seg rend="reditel">Karel Oliva</seg>
						<seg rend="jmeno">Director of the Institute of the Czech Language AS CR</seg>
					</cell>
					<cell>
						<seg rend="jmeno">Jiří Padevět</seg>
						<seg rend="reditel">Director of Academia Publishing House</seg>
					</cell>
				</row>
			</table>
		</div>
	</xsl:template>

	<xsl:template name="venovani">
		<div type="venovani">
			<p rend="nazev">Výbor ze starší české literatury</p>
			<p rend="nazev2">oddělení vývoje jazyka Ústavu pro jazyk český AV ČR, v. v. i.</p>
			<p rend="vychazi">Publikace vychází v Nakladatelství Academia ke stému výročí Ústavu pro jazyk český</p>
							<figure subtype="A4"><graphic url="100let_venovani_A4.png" /></figure>
				<figure subtype="A6"><graphic url="100let_venovani_A6.png" /></figure>

		<!--	<xsl:call-template name="Nemcove" />-->
				<xsl:call-template name="Necasovi" />
			<table rend="podpisy">
				<row>
					<cell>
						<seg><figure subtype="A4"><graphic url="Podpis_Oliva.png" /></figure>
							<figure subtype="A6"><graphic url="Podpis_Oliva_A6.png" /></figure>
						</seg>
						<seg rend="reditel">ředitel Ústavu pro jazyk český AV ČR, v. v. i.</seg>
						<seg rend="jmeno">doc. RNDr. Karel Oliva, Dr.</seg>
					</cell>
					<cell>
						<seg><figure subtype="A4"><graphic url="Podpis_Padevet.png" /></figure>
							<figure subtype="A6"><graphic url="Podpis_Padevet_A6.png" /></figure>
						</seg>
			<seg rend="reditel">ředitel Nakladatelství Academia</seg>
			<seg rend="jmeno">Jiří Padevět</seg>
					</cell>
				</row>
			</table>
		</div>
	</xsl:template>
	

<xsl:template name="Necasovi">
		<p rend="komu">Tuto elektronickou knihu věnujeme předsedovi vlády České republiky RNDr. Petru Nečasovi</p>
	</xsl:template>

	<xsl:template name="Nemcove">
		<p rend="komu">Tuto elektronickou knihu věnujeme předsedkyni Poslanecké sněmovny Parlamentu České republiky Miroslavě Němcové</p>
	</xsl:template>

	<xsl:template name="copyright">
		<xsl:param name="vroceni" />
		<div type="editorial">
			<p>Copyright © <xsl:apply-templates select="ev:Hlavicka/ev:EditoriPrepisu" mode="copyright"/><xsl:text>, </xsl:text><xsl:value-of select="$vroceni"/></p>
		</div>
		<div type="editorial">
			<p>Copyright © oddělení vývoje jazyka Ústavu pro jazyk český AV ČR, v. v. i., <xsl:value-of select="$vroceni"/></p>
		</div>
		<div type="editorial">
			<p>Copyright © Nakladatelství Academia, Středisko společných činností AV ČR, v. v. i., <xsl:value-of select="$vroceni"/></p>
		</div>

	</xsl:template>

	<xsl:template match="ev:EditoriPrepisu" mode="copyright">
		<xsl:apply-templates select="ev:string" mode="copyright"/>
	</xsl:template>

	<xsl:template match="ev:string" mode="copyright">
		<xsl:call-template name="jmenoCloveka"/>
		<xsl:if test="not(position() = last())">
			<xsl:text>, </xsl:text>
		</xsl:if>
	</xsl:template>

	<xsl:template match="ev:Hlavicka"/>
	<xsl:template match="ev:Hlavicka[contains(ev:TypPredlohy, 'Rukopis')]">
		<div type="editorial">
			<p> Předloha: <xsl:value-of select="ev:TypPredlohyText"/><xsl:text>, </xsl:text>
				<xsl:value-of select="ev:ZemeUlozeni"/><xsl:text>, </xsl:text>
				<xsl:value-of select="ev:MestoUlozeni"/><xsl:text>, </xsl:text>
				<xsl:value-of select="ev:InstituceUlozeni"/><xsl:text>, sign.: </xsl:text>
				<xsl:value-of select="ev:Signatura"/><xsl:text>, </xsl:text>
				<xsl:value-of select="ev:FoliacePaginace"/>
			</p>
		</div>
	</xsl:template>
	<xsl:template match="ev:Hlavicka[contains(ev:TypPredlohy, 'StaryTisk')]">
		<div type="editorial">
			<p>
				<xsl:text>Vytiskl </xsl:text>
				<xsl:value-of select="ev:Tiskar"/>
				<xsl:text>, </xsl:text>
				<xsl:value-of select="ev:MistoTisku"/>
				<xsl:text> </xsl:text>
				<xsl:value-of select="ev:DataceText"/>
			</p>
		</div>
	</xsl:template>

	<xsl:template match="ev:EditoriPrepisu">
		<div type="editorial">
			<p>Elektronickou edici připravili: <xsl:apply-templates select="ev:string"/><xsl:text>Boris Lehečka (kódování TEI)</xsl:text></p>
		</div>
	</xsl:template>

	<xsl:template match="ev:string"/>

	<xsl:template match="ev:EditoriPrepisu/ev:string">
		<xsl:call-template name="jmenoCloveka"/>
		<xsl:choose>
			<xsl:when test="contains(.,'Černá')">
				<xsl:text> (hlavní editorka), </xsl:text>
			</xsl:when>
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
		<docAuthor>
			<xsl:value-of select="."/>
		</docAuthor>
	</xsl:template>
</xsl:stylesheet>
