<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" version="1.0">
  <xsl:template match="Nadpis">
    <xsl:element name="head"/>
    <xsl:apply-templates/>
  </xsl:template>
  <xsl:template match="Titul">
    <xsl:element name="head"/>
    <xsl:apply-templates/>
  </xsl:template>
  <xsl:template match="Bible_Nadpis_kapitoly">
    <xsl:element name="head"/>
    <xsl:apply-templates/>
  </xsl:template>
  <xsl:template match="Podnadpis">
    <xsl:element name="head1"/>
    <xsl:apply-templates/>
  </xsl:template>
  <xsl:template match="Podnadpis">
    <xsl:element name="label"/>
    <xsl:apply-templates/>
  </xsl:template>
  <xsl:template match="Normální">
    <xsl:element name="p"/>
    <xsl:apply-templates/>
  </xsl:template>
  <xsl:template match="Volny_radek">
    <xsl:element name="p"/>
    <xsl:apply-templates/>
  </xsl:template>
  <xsl:template match="Vers">
    <xsl:element name="l"/>
    <xsl:apply-templates/>
  </xsl:template>
  <xsl:template match="Polozka_rejstriku">
    <xsl:element name="p"/>
    <xsl:apply-templates/>
  </xsl:template>
  <xsl:template match="bible_zkratka_knihy">
    <xsl:element name="supplied"/>
    <xsl:attribute name="n">
      <xsl:value-of select="."/>
    </xsl:attribute>
    <xsl:attribute name="reason">
      <xsl:text>bible</xsl:text>
    </xsl:attribute>
    <xsl:apply-templates/>
  </xsl:template>
  <xsl:template match="bible_cislo_kapitoly">
    <xsl:element name="supplied"/>
    <xsl:attribute name="n">
      <xsl:value-of select="."/>
    </xsl:attribute>
    <xsl:attribute name="reason">
      <xsl:text>bible</xsl:text>
    </xsl:attribute>
    <xsl:apply-templates/>
  </xsl:template>
  <xsl:template match="bible_cislo_verse">
    <xsl:element name="supplied"/>
    <xsl:attribute name="n">
      <xsl:value-of select="."/>
    </xsl:attribute>
    <xsl:attribute name="reason">
      <xsl:text>bible</xsl:text>
    </xsl:attribute>
    <xsl:apply-templates/>
  </xsl:template>
  <xsl:template match="bible_cislo_verse">
    <xsl:element name="supplied"/>
    <xsl:attribute name="n">
      <xsl:value-of select="."/>
    </xsl:attribute>
    <xsl:attribute name="reason">
      <xsl:text>bible</xsl:text>
    </xsl:attribute>
    <xsl:apply-templates/>
  </xsl:template>
  <xsl:template match="Text">
    <xsl:element name="text"/>
    <xsl:apply-templates/>
  </xsl:template>
  <xsl:template match="Standardní písmo odstavce">
    <xsl:element name="text"/>
    <xsl:apply-templates/>
  </xsl:template>
  <xsl:template match="NormalCharacter">
    <xsl:element name="text"/>
    <xsl:apply-templates/>
  </xsl:template>
  <xsl:template match="cislo_verse">
    <xsl:element name="lb"/>
    <xsl:attribute name="n">
      <xsl:value-of select="."/>
    </xsl:attribute>
    <xsl:apply-templates/>
  </xsl:template>
  <xsl:template match="foliace">
    <xsl:element name="pb"/>
    <xsl:attribute name="n">
      <xsl:value-of select="."/>
    </xsl:attribute>
    <xsl:apply-templates/>
  </xsl:template>
  <xsl:template match="pramen">
    <xsl:element name="sic"/>
    <xsl:apply-templates/>
  </xsl:template>
  <xsl:template match="pramen_horni_index">
    <xsl:element name="hi"/>
    <xsl:attribute name="rend">
      <xsl:text>superscript</xsl:text>
    </xsl:attribute>
    <xsl:apply-templates/>
  </xsl:template>
  <xsl:template match="emendace">
    <xsl:element name="corr"/>
    <xsl:apply-templates/>
  </xsl:template>
  <xsl:template match="doplneny_text">
    <xsl:element name="supplied"/>
    <xsl:apply-templates/>
  </xsl:template>
  <xsl:template match="poznamka_pod_carou">
    <xsl:element name="note"/>
    <xsl:apply-templates/>
  </xsl:template>
  <xsl:template match="poznamka">
    <xsl:element name="note"/>
    <xsl:apply-templates/>
  </xsl:template>
  <xsl:template match="poznamka_kurziva">
    <xsl:element name="kurziva"/>
    <xsl:apply-templates/>
  </xsl:template>
  <xsl:template match="poznamka_horni_index">
    <xsl:element name="note"/>
    <xsl:attribute name="rend">
      <xsl:text>sup</xsl:text>
    </xsl:attribute>
    <xsl:apply-templates/>
  </xsl:template>
  <xsl:template match="polozka">
    <xsl:element name="item"/>
    <xsl:apply-templates/>
  </xsl:template>
  <xsl:template match="cizi_jazyk">
    <xsl:element name="foreign"/>
    <xsl:apply-templates/>
  </xsl:template>
  <xsl:template match="cizi_jazyk_doplneny_text">
    <xsl:element name="supplied"/>
    <xsl:apply-templates/>
  </xsl:template>
  <xsl:template match="cizi_jazyk_horni_index">
    <xsl:element name="hi"/>
    <xsl:attribute name="rend">
      <xsl:text>superscript</xsl:text>
    </xsl:attribute>
    <xsl:apply-templates/>
  </xsl:template>
  <xsl:template match="pripisek_marginalni_mladsi">
    <xsl:element name="add"/>
    <xsl:attribute name="place">
      <xsl:text>margin</xsl:text>
    </xsl:attribute>
    <xsl:attribute name="type">
      <xsl:text>non-contemporaneous</xsl:text>
    </xsl:attribute>
    <xsl:apply-templates/>
  </xsl:template>
  <xsl:template match="pripisek_marginalni_soucasny">
    <xsl:element name="add"/>
    <xsl:attribute name="place">
      <xsl:text>margin</xsl:text>
    </xsl:attribute>
    <xsl:attribute name="type">
      <xsl:text>contemporaneous</xsl:text>
    </xsl:attribute>
    <xsl:apply-templates/>
  </xsl:template>
  <xsl:template match="pripisek_interlinearni_mladsi">
    <xsl:element name="add"/>
    <xsl:attribute name="place">
      <xsl:text>inline</xsl:text>
    </xsl:attribute>
    <xsl:attribute name="type">
      <xsl:text>non-contemporaneous</xsl:text>
    </xsl:attribute>
    <xsl:apply-templates/>
  </xsl:template>
	
	<xsl:template match="pripisek_interlinearni_mladsi_cizi_jazyk">
		<xsl:element name="add"/>
		<xsl:attribute name="place">
			<xsl:text>inline</xsl:text>
		</xsl:attribute>
		<xsl:attribute name="type">
			<xsl:text>non-contemporaneous</xsl:text>
		</xsl:attribute>
		<xsl:element name="foreign">
			<xsl:apply-templates/>
		</xsl:element>
	</xsl:template>
	
  <xsl:template match="pripisek_interlinearni_soucasny">
    <xsl:element name="add"/>
    <xsl:attribute name="place">
      <xsl:text>inline</xsl:text>
    </xsl:attribute>
    <xsl:attribute name="type">
      <xsl:text>contemporaneous</xsl:text>
    </xsl:attribute>
    <xsl:apply-templates/>
  </xsl:template>
  <xsl:template match="torzo">
    <xsl:element name="torzo"/>
    <xsl:apply-templates/>
  </xsl:template>
  <xsl:template match="Hlavicka"/>
  <xsl:template match="delimitator_ekvivalentu"/>
<!--  <xsl:template match="Elegantní&#32;tabulka"/>-->
  <xsl:template match="interni_poznamka"/>
  <xsl:template match="interni_poznamka_kurziva"/>
	<xsl:template match="interni_poznamka_horni_index" />
  <xsl:template match="hyperlemma"/>
  <xsl:template match="lemma"/>
  <xsl:template match="pripisek"/>
  <xsl:template match="transliterace"/>
  <xsl:template match="transliterace_rozepsani_zkratky"/>
  <xsl:template match="transliterace_cizi_jazyk"/>
</xsl:stylesheet>
