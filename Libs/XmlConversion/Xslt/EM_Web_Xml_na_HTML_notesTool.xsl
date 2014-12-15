<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
	xmlns:xd="http://www.oxygenxml.com/ns/doc/xsl"
	exclude-result-prefixes="xd"
	version="1.0">
	<xsl:output method="html" indent="yes" encoding="UTF-8" />
	<xd:doc scope="stylesheet">
		<xd:desc>
			<xd:p><xd:b>Created on:</xd:b> Dec 22, 2010</xd:p>
			<xd:p><xd:b>Author:</xd:b> boris</xd:p>
			<xd:p>Šablona pro generování poznámkového aparátu (emendací, edičních komentářů) pro webovou stránku edičního modulu.</xd:p>
		</xd:desc>
	</xd:doc>
	
	

	<xd:doc>
		<xd:desc><xd:p>Cesta k ikoně pro zavření oddílu s poznámkovým aparátem. Bude potřeba změnit, když se změní (virtuální) cesta k ikoně.</xd:p></xd:desc>
	</xd:doc>
	<xsl:variable name="ikonaProZavreni" select="'/Devedu.Ujc.Web/Images/close_ico.jpg'" />
	

	<xd:doc>
		<xd:desc>
			<xd:p>Začíná se u kořenového elemetnu dokumentu. Předpokládá se, že kořenový element se bude jmenovat <xd:b>xmls</xd:b>.</xd:p>
		</xd:desc>
	</xd:doc>
	
	<xsl:template match="/">
			<xsl:apply-templates select="/" mode="poznamky" />
	</xsl:template>

<xd:doc scope="component">
	<xd:desc><xd:p>Šablony pro generování oddílu s poznámkovým aparátem. Šablony jsou označeny modem <xd:b>poznamka</xd:b>.</xd:p> </xd:desc>
</xd:doc>
	
	<xsl:template match="xmls" mode="poznamky">
		<!-- vygeneruje se oddíl pro emendace a poznámkový aparát -->
		<div id="notesTool">
			<table width="100%">
				<colgroup>
					<col width="10%" />
					<col width="90%" />
				</colgroup>
				<tr>
					<td colspan="2" style="text-align: right;">
						<a href="#" class="closeNotesTool">
							<!-- pro soubor s obrázkem se používá proměnná -->
							<img src="{$ikonaProZavreni}" alt="X" /></a>
					</td>
				</tr>
				<!-- nejdříve se generují řádky s emendacemi -->
				<xsl:apply-templates mode="poznamky" select="//note[choice]" />
				<!-- následují řádky s edičním komentářem -->
				<xsl:apply-templates mode="poznamky" select="//note[text()]" />
			</table>
		</div>
		</xsl:template>


	<xd:doc>
		<xd:desc><xd:p>Šablona pro poznámky, které obsahují pouze prvek <xd:b>choice</xd:b>, tj. pro emendace.</xd:p></xd:desc>
	</xd:doc>
	<xsl:template match="note[choice]" mode="poznamky">
		<!-- TODO: pro zvýraznění řádku s emendací je potřeba přiřadit řádku (?) identifikátor -->
		<tr>
			<!-- číslování poznámky se generuje automaticky ze všech komentářů s emendacemi v aktuálním dokumentu XML -->
			<!-- emendace se číslují pomocí malých písmen latinské abecedy -->
			<td><xsl:number level="any" count="note[choice]" format="a"/></td>
			<td>
				<span class="italic"><xsl:apply-templates select="choice/corr" /></span>] <xsl:apply-templates select="choice/sic"/>
			</td>
		</tr>
	</xsl:template>
	

<xd:doc>
	<xd:desc><xd:p>Šablona pro poznámky, které obsahují pouze text, tj. pro ediční komentáře.</xd:p></xd:desc>
</xd:doc>
	<xsl:template match="note[text()]" mode="poznamky">
		<!--TODO: pro zvýraznění řádku s edičním komentářem je potřeba přiřadit řádku (?) identifikátor -->
		<tr>
			<!-- číslování poznámky se generuje automaticky ze všech edičních komentářů v aktuálním dokumentu XML -->
			<!-- ediční komentáře se číslují pomocí arabských číslic -->
			<td><xsl:number level="any" count="note[text()]" format="1"/></td>
			<td>
				<xsl:apply-templates select="text()" />
			</td>
		</tr>
	</xsl:template>
	

</xsl:stylesheet>