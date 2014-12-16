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
			<xd:p>Šablona pro generování hlavního textu elektronické edice pro webovou stránku edičního modulu.</xd:p>
		</xd:desc>
	</xd:doc>
	
	
	<xd:doc>
		<xd:desc>
			<xd:p>Začíná se u kořenového elemetnu dokumentu. Předpokládá se, že kořenový element se bude jmenovat <xd:b>xmls</xd:b>.</xd:p>
		</xd:desc>
	</xd:doc>
	
	<xsl:template match="/">
						<xsl:apply-templates select="xmls" />
	</xsl:template>
	
	<xsl:template match="xmls">
		<xsl:apply-templates />
	</xsl:template>
	
	<xd:doc>
		<xd:desc><xd:p>Šablona pro ediční komentář ke konkrétní elektronické edici.</xd:p> </xd:desc>
	</xd:doc>
	
	<xsl:template match="div[@type='editorial' and @subtype='comment']">
		<xsl:element name="div">
			<xsl:attribute name="style"><xsl:text>editorial</xsl:text></xsl:attribute>
			<xsl:apply-templates />
		</xsl:element>
	</xsl:template>

	<xd:doc>
		<xd:desc><xd:p>Šablona pro biblickou knihu.</xd:p> </xd:desc>
	</xd:doc>
	<xsl:template match="div[@type='bible' and @subtype='book']">
		<xsl:element name="div">
			<xsl:attribute name="style"><xsl:text>book</xsl:text></xsl:attribute>
			<xsl:apply-templates />
		</xsl:element>
	</xsl:template>

	<xd:doc>
		<xd:desc><xd:p>Šablona pro biblickou kapitolu</xd:p> </xd:desc>
	</xd:doc>
	<xsl:template match="div[@type='bible' and @subtype='chapter']">
		<xsl:element name="div">
			<xsl:attribute name="style"><xsl:text>chapter</xsl:text></xsl:attribute>
			<xsl:apply-templates />
		</xsl:element>
	</xsl:template>

	<xd:doc>
		<xd:desc><xd:p>Šablona pro oddíl původního textu</xd:p> </xd:desc>
	</xd:doc>
	
	<xsl:template match="div">
		<xsl:element name="div">
			<xsl:apply-templates />
		</xsl:element>
	</xsl:template>
	
	<xd:doc>
		<xd:desc><xd:p>Šablona pro seznam typu rejstřík.</xd:p> </xd:desc>
	</xd:doc>
	
	<xsl:template match="list[@type='index']">
		<xsl:element name="ul">
			<xsl:attribute name="style"><xsl:text>index</xsl:text></xsl:attribute>
			<xsl:apply-templates />
		</xsl:element>
	</xsl:template>

	<xd:doc>
		<xd:desc><xd:p>Šablona pro nadpis.</xd:p> </xd:desc>
	</xd:doc>
	
	<xsl:template match="head">
		<xsl:element name="h3">
			<xsl:apply-templates />
		</xsl:element>
	</xsl:template>

	<xd:doc>
		<xd:desc><xd:p>Šablona pro ediční poznámku (obsahující pouze text). V hlavním textu se vytvoří pouze odkaz s číslem poznámky.</xd:p> </xd:desc>
	</xd:doc>
	
	<xsl:template match="note[text()]">
		<a href="#" class="collapseNotesTool">
			<span class="note">[<xsl:number level="any"  count="note[text()]" format="1"/>]</span>
			</a>
	</xsl:template>

	<xd:doc>
		<xd:desc><xd:p>Šablona pro emendační poznámku. V hlavním textu se vytvoří pouze odkaz s malým písmenem abecedy.</xd:p> </xd:desc>
	</xd:doc>
	<xsl:template match="note[choice]">
		<a href="#" class="collapseNotesTool">
			<span class="note">[<xsl:number level="any"  count="note[choice]" format="a"/>]</span>
		</a>
	</xsl:template>



	<xd:doc scope="component">
		<xd:desc><xd:p>Dočasné řešení</xd:p></xd:desc>
	</xd:doc>
	
	<xd:doc>
		<xd:desc><xd:p>Šablona pro hranice stran originálního textu.</xd:p></xd:desc>
	</xd:doc>
		<xsl:template match="pb">
		<xsl:element name="span">
			<xsl:attribute name="style"><xsl:text>pb</xsl:text></xsl:attribute>
		<xsl:text>|</xsl:text>
				<xsl:value-of select="@n"/>
		<xsl:text>|</xsl:text>
		</xsl:element>
	</xsl:template>

	<xd:doc>
		<xd:desc><xd:p>Šablona pro položku v seznamu (rejstříku).</xd:p></xd:desc>
	</xd:doc>
	<xsl:template match="list/item">
		<xsl:element name="li">
			<xsl:apply-templates />
		</xsl:element>
	</xsl:template>
	
	<xd:doc>
		<xd:desc><xd:p>Šablona pro odstavec.</xd:p></xd:desc>
	</xd:doc>
	
	<xsl:template match="p">
		<xsl:copy>
			<xsl:apply-templates />
		</xsl:copy>
	</xsl:template>
	

	<xd:doc>
		<xd:desc><xd:p>Šablona pro zvýraznění textu.</xd:p></xd:desc>
	</xd:doc>
	
	<xsl:template match="hi">
		<xsl:element name="span">
			<xsl:attribute name="class"><xsl:value-of select="@rend"/></xsl:attribute>
			<xsl:apply-templates />
		</xsl:element>
	</xsl:template>

	<xd:doc>
		<xd:desc><xd:p>Šablona pro kapitálku (obvykle první písmeno v knize, kapitole nebo odstavci).</xd:p></xd:desc>
	</xd:doc>
	
	<xsl:template match="c">
		<span class="initial">
			<xsl:apply-templates />
			</span>
	</xsl:template>

	<xd:doc>
		<xd:desc><xd:p>Šablona pro přípisek.</xd:p></xd:desc>
	</xd:doc>
	
	<!-- TODO: rozlišit různé typy přípisků (soudobý, orientátor ap.). -->
<xsl:template match="add">
	<a class="info" href="#">{<xsl:apply-templates />}<span>přípisek do původnícho textu</span></a>
</xsl:template>

	<xd:doc>
		<xd:desc><xd:p>Šablona pro text v cizím jazyce (ne česky).</xd:p></xd:desc>
	</xd:doc>
	
	<xsl:template match="foreign">
		<a class="info foreign" href="#"><xsl:apply-templates /><span>cizojazyčný text</span></a> 
	</xsl:template>
	
	<xd:doc>
		<xd:desc><xd:p>Šablona pro vynechanou část textu</xd:p></xd:desc>
	</xd:doc>
	<!-- TODO: spojit značku pro vynechání text s předchozím nebo následuícím textem a toto vše označit ako torzovité slovo -->
	<xsl:template match="gap">
		<a class="info fragment" href="#">…<span>torzovité slovo</span></a>
	</xsl:template>
	
</xsl:stylesheet>