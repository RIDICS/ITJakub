<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:xd="http://www.oxygenxml.com/ns/doc/xsl" xmlns:xml="http://www.w3.org/XML/1998/namespace" exclude-result-prefixes="xd" version="1.0">
	<xd:doc scope="stylesheet">
		<xd:desc>
			<xd:p><xd:b>Created on:</xd:b> Dec 3, 2010</xd:p>
			<xd:p><xd:b>Author:</xd:b> boris</xd:p>
			<xd:p/>
		</xd:desc>
	</xd:doc>

	<xsl:include href="Prevod_stylu_na_TEI.xsl"/>
	<xsl:include href="EM+EB_Prevod_stylu_na_TEI_spolecne.xsl"/>
	<xsl:output omit-xml-declaration="no" indent="yes"/>
	<xsl:strip-space elements="*"/>

	<xsl:template match="/">
		<xsl:comment> EM_Prevod_stylu_na_TEI </xsl:comment>
		<xsl:apply-templates/>
	</xsl:template>

	<xsl:template match="poznamka_pod_carou" priority="5">
		<xsl:element name="note">
			<xsl:attribute name="n">
				<xsl:value-of select="@n"/>
			</xsl:attribute>
			<xsl:attribute name="place">
				<xsl:text>bottom</xsl:text>
			</xsl:attribute>
			<xsl:apply-templates select="footnote_text | Pozn._p._č."/>
		</xsl:element>
	</xsl:template>

	<xsl:template match="footnote_text">
		<xsl:apply-templates/>
	</xsl:template>

	<xsl:template match="Nadpis_1">
		<xsl:element name="head1">
			<xsl:apply-templates/>
		</xsl:element>
	</xsl:template>

	<xsl:template match="Nadpis_2">
		<xsl:element name="head">
			<xsl:apply-templates/>
		</xsl:element>
	</xsl:template>

	<xsl:template match="bezpatkove">
		<xsl:element name="hi">
			<xsl:attribute name="rend">
				<xsl:text>sans</xsl:text>
			</xsl:attribute>
			<xsl:apply-templates/>
		</xsl:element>
	</xsl:template>

	<xsl:template match="Popisek_obrazku">
		<xsl:element name="p">
			<xsl:apply-templates/>
		</xsl:element>
	</xsl:template>

	<xsl:template match="Tucne">
		<xsl:element name="hi">
			<xsl:attribute name="rend">
				<xsl:text>bold</xsl:text>
			</xsl:attribute>
			<xsl:apply-templates/>
		</xsl:element>
	</xsl:template>

	<xsl:template match="Kurziva">
		<xsl:element name="hi">
			<xsl:attribute name="rend">
				<xsl:text>italic</xsl:text>
			</xsl:attribute>
			<xsl:apply-templates/>
		</xsl:element>
	</xsl:template>

	<xsl:template match="Pripadova_studie">
		<xsl:element name="p">
			<xsl:attribute name="rend">
				<xsl:text>studie</xsl:text>
			</xsl:attribute>
			<xsl:apply-templates/>
		</xsl:element>
	</xsl:template>

	<xsl:template match="Obrazek">
		<xsl:element name="figure">
			<xsl:element name="graphic">
				<xsl:attribute name="url">
					<xsl:value-of select="@soubor"/>
				</xsl:attribute>
			</xsl:element>
		</xsl:element>
	</xsl:template>

	<xsl:template match="Hyperlink">
		<xsl:element name="ref">
			<xsl:attribute name="target">
				<xsl:choose>
					<xsl:when test="starts-with(., 'www.')">
						<xsl:value-of select="concat('http://', .)"/>
					</xsl:when>
					<xsl:otherwise>
						<xsl:apply-templates/>
					</xsl:otherwise>
				</xsl:choose>
			</xsl:attribute>
			<xsl:apply-templates/>
		</xsl:element>
	</xsl:template>

	<xsl:template match="List_Paragraph">
		<xsl:element name="p">
			<xsl:attribute name="rend">
				<xsl:text>fajfka</xsl:text>
			</xsl:attribute>
			<xsl:apply-templates/>
		</xsl:element>
	</xsl:template>

	<xsl:template match="Tip">
		<xsl:element name="p">
			<xsl:attribute name="rend">
				<xsl:text>tip</xsl:text>
			</xsl:attribute>
			<xsl:apply-templates/>
		</xsl:element>
	</xsl:template>

	<xsl:template match="Poznamka">
		<xsl:element name="p">
			<xsl:attribute name="rend">
				<xsl:text>poznamka</xsl:text>
			</xsl:attribute>
			<xsl:apply-templates/>
		</xsl:element>
	</xsl:template>

	<xsl:template match="footnote_reference"/>
	<xsl:template match="Footnote"/>
	<xsl:template match="Pozn._p._č.">
		<xsl:apply-templates/>
	</xsl:template>

	<xsl:template match="_x0031_._Název_kapitoly">
		<xsl:element name="head1">
			<xsl:apply-templates/>
		</xsl:element>
	</xsl:template>


	<xsl:template match="_x0032_._x.x">
		<xsl:element name="head">
			<xsl:apply-templates/>
		</xsl:element>
	</xsl:template>


	<xsl:template match="_x0033_._x.x.x">
		<xsl:element name="head3">
			<xsl:apply-templates/>
		</xsl:element>
	</xsl:template>

	<xsl:template match="Normální_zkraje">
		<xsl:element name="p">
			<xsl:apply-templates/>
		</xsl:element>
	</xsl:template>


	<xsl:template match="V1_x003A__Začátek_číslo__Výčty_">
		<xsl:element name="item">
			<xsl:apply-templates/>
		</xsl:element>
	</xsl:template>


	<xsl:template match="V1_x003A__Začátek_číslo_vysvětlivka__Výčty_">
		<xsl:element name="item">
			<xsl:attribute name="type">
				<xsl:text>vysvetlivka</xsl:text>
			</xsl:attribute>
			<xsl:apply-templates/>
		</xsl:element>
	</xsl:template>


	<xsl:template match="V2_x003A__Text_číslo__Výčty_">
		<xsl:element name="item">
			<xsl:apply-templates/>
		</xsl:element>
	</xsl:template>

	<xsl:template match="V2_x003A__Text__Výčty_">
		<xsl:element name="item">
			<xsl:apply-templates/>
		</xsl:element>
	</xsl:template>

	<xsl:template match="V3_x003A__Konec_číslo__Výčty_">
		<xsl:element name="item">
			<xsl:apply-templates/>
		</xsl:element>
	</xsl:template>

	<xsl:template match="V3_x003A__Konec_číslo_vysvětlivka__Výčty_">
		<xsl:element name="item">
			<xsl:attribute name="type">
				<xsl:text>vysvetlivka</xsl:text>
			</xsl:attribute>
			<xsl:apply-templates/>
		</xsl:element>
	</xsl:template>

	<xsl:template match="V2_x003A__Text_číslo_vysvětlivka__Výčty_">
		<xsl:element name="item">
			<xsl:attribute name="type">
				<xsl:text>vysvetlivka</xsl:text>
			</xsl:attribute>
			<xsl:apply-templates/>
		</xsl:element>
	</xsl:template>

	<xsl:template match="Příklad">
		<xsl:element name="p">
			<xsl:attribute name="rend">
				<xsl:text>example</xsl:text>
			</xsl:attribute>
			<xsl:apply-templates/>
		</xsl:element>
	</xsl:template>

	<xsl:template match="Vysvětlivka">
		<xsl:element name="p">
			<xsl:attribute name="rend">
				<xsl:text>explanation</xsl:text>
			</xsl:attribute>
			<xsl:apply-templates/>
		</xsl:element>
	</xsl:template>

	<xsl:template match="Strong">
		<xsl:element name="hi">
			<xsl:attribute name="rend">
				<xsl:text>bold</xsl:text>
			</xsl:attribute>
			<xsl:apply-templates/>
		</xsl:element>
	</xsl:template>
	<xsl:template match="Emphasis">
		<xsl:element name="hi">
			<xsl:attribute name="rend">
				<xsl:text>italic</xsl:text>
			</xsl:attribute>
			<xsl:apply-templates/>
		</xsl:element>
	</xsl:template>


	<!-- Sebedůvěra -->

	<xsl:template match="_No_Paragraph_Style_">
		<xsl:element name="p">
			<xsl:attribute name="rend">
				<xsl:text>_No_Paragraph_Style_</xsl:text>
			</xsl:attribute>
			<xsl:apply-templates/>
		</xsl:element>
	</xsl:template>
	<xsl:template match="b__16777216_p_Myriad_Pro_Semibold_Condensed">
		<xsl:element name="hi">
			<xsl:attribute name="rend">
				<xsl:text>b__16777216_p_Myriad_Pro_Semibold_Condensed</xsl:text>
			</xsl:attribute>
			<xsl:apply-templates/>
		</xsl:element>
	</xsl:template>
	<xsl:template match="Bez_seznamu">
		<xsl:element name="p">
			<xsl:attribute name="rend">
				<xsl:text>Bez_seznamu</xsl:text>
			</xsl:attribute>
			<xsl:apply-templates/>
		</xsl:element>
	</xsl:template>
	<xsl:template match="citát">
		<xsl:element name="p">
			<xsl:attribute name="rend">
				<xsl:text>citat</xsl:text>
			</xsl:attribute>
			<xsl:apply-templates/>
		</xsl:element>
	</xsl:template>
	<xsl:template match="citát_autor">
		<xsl:element name="p">
			<xsl:attribute name="rend">
				<xsl:text>citat_autor</xsl:text>
			</xsl:attribute>
			<xsl:apply-templates/>
		</xsl:element>
	</xsl:template>
	<xsl:template match="Článek/oddíl">
		<xsl:element name="p">
			<xsl:attribute name="rend">
				<xsl:text>Clanek-oddil</xsl:text>
			</xsl:attribute>
			<xsl:apply-templates/>
		</xsl:element>
	</xsl:template>
	<xsl:template match="dopor___tečka">
		<xsl:element name="p">
			<xsl:attribute name="rend">
				<xsl:text>dopor___tecka</xsl:text>
			</xsl:attribute>
			<xsl:call-template name="ctverecekSpan"/>
			<xsl:apply-templates/>
		</xsl:element>
	</xsl:template>
	<xsl:template name="ctverecekSpan">
		<xsl:element name="hi">
			<xsl:attribute name="rend">
				<xsl:text>ctverecek</xsl:text>
			</xsl:attribute>
		<xsl:text>■  </xsl:text>
		</xsl:element>
	</xsl:template>
	<xsl:template match="dopor___text">
		<xsl:element name="p">
			<xsl:attribute name="rend">
				<xsl:text>dopor___text</xsl:text>
			</xsl:attribute>
			<xsl:apply-templates/>
		</xsl:element>
	</xsl:template>
	<xsl:template match="doporučení">
		<xsl:element name="p">
			<xsl:attribute name="rend">
				<xsl:text>doporuceni</xsl:text>
			</xsl:attribute>
			<xsl:apply-templates/>
		</xsl:element>
	</xsl:template>
	<!--<xsl:template match="kurziva"> <xsl:element name="hi">  <xsl:attribute name="rend"><xsl:text>kurziva</xsl:text></xsl:attribute>    <xsl:apply-templates /> </xsl:element></xsl:template>-->
	<xsl:template match="kurziva_p_Myriad_Pro">
		<xsl:element name="hi">
			<xsl:attribute name="rend">
				<xsl:text>kurziva_p_Myriad_Pro</xsl:text>
			</xsl:attribute>
			<xsl:apply-templates/>
		</xsl:element>
	</xsl:template>
	<xsl:template match="kurziva_v_10">
		<xsl:element name="hi">
			<xsl:attribute name="rend">
				<xsl:text>kurziva_v_10</xsl:text>
			</xsl:attribute>
			<xsl:apply-templates/>
		</xsl:element>
	</xsl:template>
	<xsl:template match="kurziva_v_11_p_Adobe_Garamond_Pro">
		<xsl:element name="hi">
			<xsl:attribute name="rend">
				<xsl:text>kurziva_v_11_p_Adobe_Garamond_Pro</xsl:text>
			</xsl:attribute>
			<xsl:apply-templates/>
		</xsl:element>
	</xsl:template>
	<!--<xsl:template match="Nadpis_2"> <xsl:element name="p">  <xsl:attribute name="rend"><xsl:text>Nadpis_2</xsl:text></xsl:attribute>    <xsl:apply-templates /> </xsl:element></xsl:template>-->
	<xsl:template match="Nadpis_3">
		<xsl:element name="head3">
			<xsl:attribute name="rend">
				<xsl:text>Nadpis_3</xsl:text>
			</xsl:attribute>
			<xsl:apply-templates/>
		</xsl:element>
	</xsl:template>
	<xsl:template match="Nadpis_casti">
		<xsl:element name="head">
			<xsl:attribute name="rend">
				<xsl:text>Nadpis_casti</xsl:text>
			</xsl:attribute>
			<xsl:apply-templates/>
		</xsl:element>
	</xsl:template>	
	<xsl:template match="Nadpis_kap">
		<xsl:element name="head1">
			<xsl:attribute name="rend">
				<xsl:text>Nadpis_kap</xsl:text>
			</xsl:attribute>
			<xsl:apply-templates/>
		</xsl:element>
	</xsl:template>
	<xsl:template match="Normální">
		<xsl:element name="p">
			<xsl:attribute name="rend">
				<xsl:text>Normální</xsl:text>
			</xsl:attribute>
			<xsl:apply-templates/>
		</xsl:element>
	</xsl:template>
	<xsl:template match="p_Wingdings_2">
		<xsl:element name="hi">
			<xsl:attribute name="rend">
				<xsl:text>p_Wingdings_2</xsl:text>
			</xsl:attribute>
			<xsl:apply-templates/>
		</xsl:element>
	</xsl:template>
	<xsl:template match="příprava_nadpis">
		<xsl:element name="p">
			<xsl:attribute name="rend">
				<xsl:text>priprava_nadpis</xsl:text>
			</xsl:attribute>
			<xsl:apply-templates/>
		</xsl:element>
	</xsl:template>
	<xsl:template match="shrnutí">
		<xsl:element name="p">
			<xsl:attribute name="rend">
				<xsl:text>shrnuti</xsl:text>
			</xsl:attribute>
			<xsl:apply-templates/>
		</xsl:element>
	</xsl:template>
	<xsl:template match="Standardní_písmo_odstavce">
		<xsl:element name="text">
			<xsl:apply-templates/>
		</xsl:element>
	</xsl:template>
	<xsl:template match="tabulka___text">
		<xsl:element name="p">
			<xsl:attribute name="rend">
				<xsl:text>tabulka___text</xsl:text>
			</xsl:attribute>
			<xsl:apply-templates/>
		</xsl:element>
	</xsl:template>
	<xsl:template match="tabulka_nadpis">
		<xsl:element name="p">
			<xsl:attribute name="rend">
				<xsl:text>tabulka_nadpis</xsl:text>
			</xsl:attribute>
			<xsl:apply-templates/>
		</xsl:element>
	</xsl:template>
	<xsl:template match="tečka">
		<xsl:element name="p">
			<xsl:attribute name="rend">
				<xsl:text>tecka</xsl:text>
			</xsl:attribute>
			<xsl:call-template name="ctverecekSpan"/>
			<xsl:apply-templates/>
		</xsl:element>
	</xsl:template>
	<xsl:template match="text_bezar">
		<xsl:element name="p">
			<xsl:attribute name="rend">
				<xsl:text>text_bezar</xsl:text>
			</xsl:attribute>
			<xsl:apply-templates/>
		</xsl:element>
	</xsl:template>
	<xsl:template match="tip">
		<xsl:element name="p">
			<xsl:attribute name="rend">
				<xsl:text>tip</xsl:text>
			</xsl:attribute>
			<xsl:apply-templates/>
		</xsl:element>
	</xsl:template>
	<xsl:template match="tucne">
		<xsl:element name="hi">
			<xsl:attribute name="rend">
				<xsl:text>tucne</xsl:text>
			</xsl:attribute>
			<xsl:apply-templates/>
		</xsl:element>
	</xsl:template>
	<xsl:template match="tucne_p_Adobe_Garamond_Pro_Bold">
		<xsl:element name="hi">
			<xsl:attribute name="rend">
				<xsl:text>tucne_p_Adobe_Garamond_Pro_Bold</xsl:text>
			</xsl:attribute>
			<xsl:apply-templates/>
		</xsl:element>
	</xsl:template>
	<xsl:template match="tucne_p_Myriad_Pro_Cond">
		<xsl:element name="hi">
			<xsl:attribute name="rend">
				<xsl:text>tucne_p_Myriad_Pro_Cond</xsl:text>
			</xsl:attribute>
			<xsl:apply-templates/>
		</xsl:element>
	</xsl:template>
	<xsl:template match="txt">
		<xsl:element name="p">
			<xsl:attribute name="rend">
				<xsl:text>txt</xsl:text>
			</xsl:attribute>
			<xsl:apply-templates/>
		</xsl:element>
	</xsl:template>
	<xsl:template match="v_10">
		<xsl:element name="hi">
			<xsl:attribute name="rend">
				<xsl:text>v_10</xsl:text>
			</xsl:attribute>
			<xsl:apply-templates/>
		</xsl:element>
	</xsl:template>
	<xsl:template match="v_10_tucne_p_Myriad_Pro_Semibold_Condensed">
		<xsl:element name="hi">
			<xsl:attribute name="rend">
				<xsl:text>v_10_tucne_p_Myriad_Pro_Semibold_Condensed</xsl:text>
			</xsl:attribute>
			<xsl:apply-templates/>
		</xsl:element>
	</xsl:template>
	<xsl:template match="v_11_b_0_p_Adobe_Garamond_Pro">
		<xsl:element name="hi">
			<xsl:attribute name="rend">
				<xsl:text>v_11_b_0_p_Adobe_Garamond_Pro</xsl:text>
			</xsl:attribute>
			<xsl:apply-templates/>
		</xsl:element>
	</xsl:template>
	<xsl:template match="v_11_p_Adobe_Garamond_Pro">
		<xsl:element name="hi">
			<xsl:attribute name="rend">
				<xsl:text>v_11_p_Adobe_Garamond_Pro</xsl:text>
			</xsl:attribute>
			<xsl:apply-templates/>
		</xsl:element>
	</xsl:template>
	<xsl:template match="v_11_tucne_p_Adobe_Garamond_Pro_Bold">
		<xsl:element name="hi">
			<xsl:attribute name="rend">
				<xsl:text>v_11_tucne_p_Adobe_Garamond_Pro_Bold</xsl:text>
			</xsl:attribute>
			<xsl:apply-templates/>
		</xsl:element>
	</xsl:template>
	<xsl:template match="v_12_tucne">
		<xsl:element name="hi">
			<xsl:attribute name="rend">
				<xsl:text>v_12_tucne</xsl:text>
			</xsl:attribute>
			<xsl:apply-templates/>
		</xsl:element>
	</xsl:template>
	<xsl:template match="v_12_tucne_b_8355711_p_Myriad_Pro_Cond">
		<xsl:element name="hi">
			<xsl:attribute name="rend">
				<xsl:text>v_12_tucne_b_8355711_p_Myriad_Pro_Cond</xsl:text>
			</xsl:attribute>
			<xsl:apply-templates/>
		</xsl:element>
	</xsl:template>
	<xsl:template match="v_12_tucne_p_Myriad_Pro_Cond">
		<xsl:element name="hi">
			<xsl:attribute name="rend">
				<xsl:text>v_12_tucne_p_Myriad_Pro_Cond</xsl:text>
			</xsl:attribute>
			<xsl:apply-templates/>
		</xsl:element>
	</xsl:template>
	<xsl:template match="v_14">
		<xsl:element name="hi">
			<xsl:attribute name="rend">
				<xsl:text>v_14</xsl:text>
			</xsl:attribute>
			<xsl:apply-templates/>
		</xsl:element>
	</xsl:template>
	<xsl:template match="v_14_tucne">
		<xsl:element name="hi">
			<xsl:attribute name="rend">
				<xsl:text>v_14_tucne</xsl:text>
			</xsl:attribute>
			<xsl:apply-templates/>
		</xsl:element>
	</xsl:template>
	<xsl:template match="v_16_tucne_p_Myriad_Pro_Cond">
		<xsl:element name="hi">
			<xsl:attribute name="rend">
				<xsl:text>v_16_tucne_p_Myriad_Pro_Cond</xsl:text>
			</xsl:attribute>
			<xsl:apply-templates/>
		</xsl:element>
	</xsl:template>
	<xsl:template match="v_24">
		<xsl:element name="hi">
			<xsl:attribute name="rend">
				<xsl:text>v_24</xsl:text>
			</xsl:attribute>
			<xsl:apply-templates/>
		</xsl:element>
	</xsl:template>
	<xsl:template match="v_24_p_Myriad_Pro_Semibold_Condensed">
		<xsl:element name="hi">
			<xsl:attribute name="rend">
				<xsl:text>v_24_p_Myriad_Pro_Semibold_Condensed</xsl:text>
			</xsl:attribute>
			<xsl:apply-templates/>
		</xsl:element>
	</xsl:template>
	<xsl:template match="v_40">
		<xsl:element name="hi">
			<xsl:attribute name="rend">
				<xsl:text>v_40</xsl:text>
			</xsl:attribute>
			<xsl:apply-templates/>
		</xsl:element>
	</xsl:template>
	<xsl:template match="v_8_5">
		<xsl:element name="hi">
			<xsl:attribute name="rend">
				<xsl:text>v_8_5</xsl:text>
			</xsl:attribute>
			<xsl:apply-templates/>
		</xsl:element>
	</xsl:template>
	<xsl:template match="v_9">
		<xsl:element name="hi">
			<xsl:attribute name="rend">
				<xsl:text>v_9</xsl:text>
			</xsl:attribute>
			<xsl:apply-templates/>
		</xsl:element>
	</xsl:template>
	<xsl:template match="v_9_b_0_p_Myriad_Pro">
		<xsl:element name="hi">
			<xsl:attribute name="rend">
				<xsl:text>v_9_b_0_p_Myriad_Pro</xsl:text>
			</xsl:attribute>
			<xsl:apply-templates/>
		</xsl:element>
	</xsl:template>
	<xsl:template match="v_9_p_Myriad_Pro_Light">
		<xsl:element name="hi">
			<xsl:attribute name="rend">
				<xsl:text>v_9_p_Myriad_Pro_Light</xsl:text>
			</xsl:attribute>
			<xsl:apply-templates/>
		</xsl:element>
	</xsl:template>
	<xsl:template match="zacatek">
		<xsl:element name="p">
			<xsl:attribute name="rend">
				<xsl:text>zacatek</xsl:text>
			</xsl:attribute>
			<xsl:apply-templates/>
		</xsl:element>
	</xsl:template>
	<xsl:template match="Znakový_styl1">
		<xsl:element name="hi">
			<xsl:attribute name="rend">
				<xsl:text>Znakovy_styl1</xsl:text>
			</xsl:attribute>
			<xsl:apply-templates/>
		</xsl:element>
	</xsl:template>
	<xsl:template match="Znakový_styl2">
		<xsl:element name="hi">
			<xsl:attribute name="rend">
				<xsl:text>Znakovy_styl2</xsl:text>
			</xsl:attribute>
			<xsl:apply-templates/>
		</xsl:element>
	</xsl:template>
	<xsl:template match="Znakový_styl3">
		<xsl:element name="hi">
			<xsl:attribute name="rend">
				<xsl:text>Znakovy_styl3</xsl:text>
			</xsl:attribute>
			<xsl:apply-templates/>
		</xsl:element>
	</xsl:template>


</xsl:stylesheet>
