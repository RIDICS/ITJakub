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
	
	<xsl:output omit-xml-declaration="no" indent="yes"/>
	<xsl:strip-space elements="*"/>
	
	<xsl:template match="/">
		<TEI xmlns="http://www.tei-c.org/ns/1.0">
			<teiHeader>
				<fileDesc>
					<titleStmt>
						<title>Slovo do světa stvořenie - vzor 2010-03-18</title>
					</titleStmt>
					<editionStmt>
						<edition>elektronická edice</edition>
						<respStmt>
							<resp>editor</resp>
							<name type="person">Černá, Alena M.</name>
						</respStmt>
						<respStmt>
							<resp>kódování TEI</resp>
							<name type="person">Lehečka, Boris</name>
						</respStmt>
					</editionStmt>
					<publicationStmt>
						<publisher>oddělení vývoje jazyka Ústavu pro jazyk český AV ČR, v. v. i.,<email>vyvoj(at)ujc.cas.cz</email> 
						</publisher>
						<publisher>Manuscriptorium.com</publisher>
						<availability status="restricted">
							<p>Tato edice je autorské dílo chráněné ve smyslu zákona č. 121/2000 Sb., o právu autorském, a je určena pouze k nekomerčním účelům.</p>
						</availability>
					</publicationStmt>
					<sourceDesc>
						<msDesc xml:id="ujcav_10" xml:lang="cs">
							<msIdentifier>
								<country key="xs">Česko</country>
								<settlement>Praha</settlement>
								<repository>Archiv Pražského hradu</repository>
								<idno>XUJC</idno>
							</msIdentifier>
							<msContents>
								<msItem>
									<title>Slovo do světa stvořenie - vzor 2010-03-18</title>
								</msItem>
							</msContents>
							<history>
								<origin>
									<origDate notBefore="1350" notAfter="1400">2. polovina 14. století</origDate>
								</origin>
							</history>
						</msDesc>
					</sourceDesc>
				</fileDesc>
				<encodingDesc>
					<editorialDecl>
						<p>Základem elektronické edice je edice Klaretova Glosáře v publikaci V. Flajšhanse Klaret a jeho družina. Sv. I. Slovníky veršované. Praha 1926.</p>
					</editorialDecl>
					<variantEncoding method="location-referenced" location="internal"/>
				</encodingDesc>
				<profileDesc>
					<handNotes>
						<handNote xml:id="korektor">soudobý korektor</handNote>
					</handNotes>
				</profileDesc>
				<revisionDesc>
					<change when="2009-12-13">Opravy chyb ve formátování zdrojového textu</change>
					<change when="2010-01-30">Oprava textu</change>
					<change when="2010-02-03">Změna značkování XML</change>
				</revisionDesc>
			</teiHeader>
			<text>
				<front>
					<docTitle>
						<titlePart>Slovo do světa stvořenie</titlePart>
					</docTitle>
					<!-- Doplněno 18. 11. 2010; atribut subtype="comment" pro ediční komentář -->
					<div type="editorial" subtype="comment">
						<p>Základem elektronické edice je edice Klaretova Glosáře v publikaci V. Flajšhanse Klaret a jeho družina. Sv. I. Slovníky veršované. Praha 1926. Text obsahuje kromě transkripce také transliteraci, a to v podobě, v jaké je uvedena ve slovníku k Flajšhanshově edici. Grafická slova obsahující současně českou i latinskou část byla na rozdíl od knižní edice v elektronické edici rozdělena. Podle edice ponecháváme v transliteraci kulaté s, nepřepisuje se na dlouhé ſ. V elektronické edici rovněž prezentujeme formou různočtení odlišná čtení jiných badatelů, spolu s zkratkovitou identifikací autora či zdroje různočtení. V elektronické edici jsme použili různočtení z těchto děl:</p>
						<list>
							<item>Bělič, J., Kamiš, A., Kučera, K.: Malý staročeský slovník. Praha 1978 (zkratka: MSS).</item>
							<item>Elektronický slovník staré češtiny. Praha od 2005 (zkratka: ESSČ).</item>
							<item>Gebauer, J.: Slovník staročeský. Praha 19702 (zkratka: GbSlov).</item>
							<item>Jílek, F.: Klaretovo české názvosloví mluvnické. Věstník Královské české společnosti nauk. Třída filosoficko-historicko-filologická. č. IV, 1950, Praha : Královská česká společnost nauk, 1951 (zkratka: Jílek 1951).</item>
						</list>
						<!-- Doplněno 18. 11. 2010 -->
						<!-- Obázek s popiskem -->
						<figure>
							<graphic url="Slovo/0001_Titulni_stranka.jpg"/>
							<p>Obrázek titulní stránky</p>
						</figure>
					</div>
					
				</front>
					<xsl:apply-templates mode="jmennyProstor" />
			</text>
			</TEI>
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
			<xsl:otherwise><xsl:value-of select="local-name()"/></xsl:otherwise>
		</xsl:choose>
		</xsl:variable>
		<xsl:attribute name="{$nazev}">
			<xsl:value-of select="."/>
		</xsl:attribute>
	</xsl:template>
	
</xsl:stylesheet>