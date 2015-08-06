<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
	xmlns:xd="http://www.oxygenxml.com/ns/doc/xsl"
	exclude-result-prefixes="xd"
	version="1.0">
	<xd:doc scope="stylesheet">
		<xd:desc>
			<xd:p><xd:b>Created on:</xd:b> Jan 28, 2011</xd:p>
			<xd:p><xd:b>Author:</xd:b> Boris</xd:p>
			<xd:p>Převede styly na prostý text, nepotřebné styly odstraní.</xd:p>
		</xd:desc>
	</xd:doc>
	
	
	<xsl:include href="Nedefinovany_element_chyba.xsl"/>
	
	
	<xsl:template match="body">
		<xsl:element name="doc">
			<xsl:apply-templates />
		</xsl:element>
	</xsl:template>
	
	<xsl:template match="text">
		<text>
			<xsl:apply-templates/>
		</text>
	</xsl:template>
	
	
	<xsl:template match="Titul">
		<xsl:element name="titul">
			<xsl:apply-templates/>
		</xsl:element>
	</xsl:template>
	
	<xsl:template match="Nadpis | Bible_Nadpis_kapitoly">
		<xsl:element name="nadpis">
			<xsl:apply-templates/>
		</xsl:element>
	</xsl:template>
	
	
	<xsl:template match="Podnadpis">
		<xsl:element name="podnadpis">
			<xsl:apply-templates/>
		</xsl:element>
	</xsl:template>
	
	
	<xsl:template match="Vers">
		<xsl:element name="vers">
			<xsl:apply-templates/>
		</xsl:element>
	</xsl:template>
	
	
	
	<xsl:template match="Normalni">
		<xsl:element name="odstavec">
			<xsl:apply-templates/>
		</xsl:element>
	</xsl:template>

	
	<xsl:template match="cizi_jazyk">
		<xsl:element name="cizi_jazyk">
			<xsl:apply-templates />
		</xsl:element>
	</xsl:template>

	
	
	<xsl:template match="doplneny_text">
		<xsl:element name="doplneny_text">
			<xsl:apply-templates/>
		</xsl:element>
	</xsl:template>
	
	
	<xsl:template match="table">
		<xsl:element name="grafika">
			<xsl:text>#</xsl:text>
		</xsl:element>
	</xsl:template>
	
	
	<xsl:template match="druhy_preklad">
		<xsl:element name="text">
			<xsl:apply-templates/>
		</xsl:element>
	</xsl:template>
	
	<xsl:template match="soudoby_korektor">
		<xsl:element name="text">
			<xsl:apply-templates/>
		</xsl:element>
	</xsl:template>
	
	<xsl:template match="torzo">
		<xsl:element name="torzo">
			<xsl:apply-templates />
		</xsl:element>
	</xsl:template>
	

	
	<xsl:template match="Polozka_rejstriku">
		<xsl:element name="odstavec">
			<xsl:attribute name="typ">
				<xsl:text>rejstřík</xsl:text>
			</xsl:attribute>
			<xsl:apply-templates />
		</xsl:element>
	</xsl:template>
	
	<xsl:template match="Incipit">
		<xsl:element name="incipit">
			<xsl:apply-templates />
		</xsl:element>
	</xsl:template>
	
	<xsl:template match="Explicit">
		<xsl:element name="explicit">
			<xsl:apply-templates />
		</xsl:element>
	</xsl:template>
	
	<xsl:template match="iniciala">
		<xsl:element name="text">
			<xsl:apply-templates />
		</xsl:element>
	</xsl:template>
	
	
	<xsl:template match="cizi_jazyk_doplneny_text | cizi_jazyk_horni_index">
		<xsl:element name="text">
			<xsl:apply-templates />
		</xsl:element>
	</xsl:template>


	<xsl:template match="Kurziva">
		<xsl:element name="text">
			<xsl:apply-templates />
		</xsl:element>
	</xsl:template>
	
	<xd:doc scope="component">
		<xd:desc>
			<xd:p>Ignorované styly</xd:p>
		</xd:desc>
	</xd:doc>
	
	<xsl:template match="Hlavicka"/>
	<xsl:template match="interni_poznamka"/>
	<xsl:template match="interni_poznamka_kurziva"/>
	<xsl:template match="pripisek_marginalni_mladsi" />
	<xsl:template match="pripisek_interlinearni_mladsi" />
	<xsl:template match="transliterace" />
	<xsl:template match="poznamka" />
	<xsl:template match="poznamka_kurziva" />
	<xsl:template match="poznamka_horni_index" />
	
	
	<xsl:template match="row" />
	<xsl:template match="cell" />
	
	<xsl:template match="relator" />
	<xsl:template match="lemma | hyperlemma" />
	<xsl:template match="delimitator_ekvivalentu" />
	<xsl:template match="bible_zkratka_knihy | bible_cislo_kapitoly | bible_cislo_verse" />
	<xsl:template match="pripisek_marginalni_soudoby | pripisek_interlinearni_soudoby" />
	<xsl:template match="emendace | pramen_horni_index | pramen | cislo_verse | foliace" />
	
	<!-- dočasné -->
	<xsl:template match="kod" />
	<xsl:template match="pripisek" />
	<xsl:template match="ruznocteni_autor" />
	<xsl:template match="ruznocteni" />
	<xsl:template match="Edicni_komentar" />
	<xsl:template match="transliterace_rozepsani_zkratky" />
	<xsl:template match="poznamka_pod_carou" />
	

	
</xsl:stylesheet>