<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet 
	xmlns:xsl="http://www.w3.org/1999/XSL/Transform" 
	xmlns:xd="http://www.oxygenxml.com/ns/doc/xsl"
	xmlns:xml="http://www.w3.org/XML/1998/namespace"
	exclude-result-prefixes="xd" version="1.0">
	<xd:doc scope="stylesheet">
		<xsl:import href="Nedefinovany_element_chyba.xsl"/>
		
		<xd:desc>
			<xd:p><xd:b>Created on:</xd:b> Dec 3, 2010</xd:p>
			<xd:p><xd:b>Author:</xd:b> Boris Lehečka</xd:p>
			<xd:p>Převodník společných stylů, využitelných v obou výstupech</xd:p>
		</xd:desc>
	</xd:doc>
	
	<xsl:param name="exportovatTransliteraci" /> <!-- 'true()' nebo 'false()' -->

	<xsl:template match="/">
		<xsl:comment> TB_Prevod_stylu_na_TEI; parameters: exportovatTransliteraci = '<xsl:value-of select="$exportovatTransliteraci"/>' </xsl:comment>
		<xsl:apply-templates/>
	</xsl:template>
	
<xsl:template match="body">
	<xsl:element name="doc">
		<xsl:apply-templates />
	</xsl:element>
</xsl:template>
	
	<xsl:template match="text">
		<xsl:element name="text">
			<xsl:apply-templates/>	
		</xsl:element>
	</xsl:template>
	
	
	<xsl:template match="Titul">
		<xsl:element name="titul">
			<xsl:apply-templates/>
		</xsl:element>
	</xsl:template>
	
	<!-- Bible_Nadpis_kapitoly - změněno v šabloně Prevod_starych_wordovskych_stylu_na_nove -->
	<xsl:template match="Nadpis">
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
	
	<xsl:template match="Polozka_rejstriku">
		<xsl:element name="odstavec">
			<xsl:attribute name="typ">
				<xsl:text>rejstřík</xsl:text>
			</xsl:attribute>
			<xsl:apply-templates />
		</xsl:element>
	</xsl:template>

	<xsl:template match="foliace">
		<xsl:element name="foliace">
			<xsl:apply-templates />
		</xsl:element>
	</xsl:template>
	
	<xsl:template match="cizi_jazyk">
		<xsl:element name="cizi_jazyk">
			<xsl:apply-templates />
		</xsl:element>
	</xsl:template>
	
	
	<xsl:template match="cizi_jazyk_doplneny_text">
		<xsl:element name="cizi_jazyk">
			<xsl:apply-templates />
		</xsl:element>
	</xsl:template>
	
	<xsl:template match="cizi_jazyk_horni_index">
		<xsl:element name="cizi_jazyk">
			<xsl:apply-templates />
		</xsl:element>
	</xsl:template>

	<xsl:template match="cislo_verse">
		<xsl:element name="cislo_verse">
			<xsl:attribute name="n">
				<xsl:value-of select="normalize-space(.)"/>
			</xsl:attribute>
		</xsl:element>
	</xsl:template>

	<xsl:template match="pramen">
		<xsl:element name="pramen">
			<xsl:value-of select="normalize-space(.)"/>
			<!--<xsl:apply-templates />-->
		</xsl:element>
	</xsl:template>

	<xsl:template match="pramen_horni_index">
		<xsl:element name="pramen">
				<xsl:value-of select="normalize-space(.)"/>
				<!--<xsl:apply-templates />-->
		</xsl:element>
	</xsl:template>



	<xsl:template match="emendace">
		<xsl:element name="emendace">
			<xsl:value-of select="normalize-space(.)"/>
			<!--<xsl:apply-templates />-->
		</xsl:element>
	</xsl:template>

	<xsl:template match="doplneny_text">
		<xsl:element name="doplneny_text">
			<xsl:apply-templates/>
		</xsl:element>
	</xsl:template>



	<xsl:template match="pripisek_marginalni_soudoby">
		<xsl:element name="text">
			<xsl:apply-templates/>
		</xsl:element>
	</xsl:template>
	
	<xsl:template match="pripisek_interlinearni_soudoby">
		<xsl:element name="text">
			<xsl:apply-templates/>
		</xsl:element>
	</xsl:template>
	
	<xsl:template match="table">
		<xsl:element name="grafika">
		<xsl:text>#</xsl:text>
		</xsl:element>
	</xsl:template>

	
	<xsl:template match="druhy_preklad">
		<xsl:element name="druhy_preklad">
			<xsl:apply-templates/>
		</xsl:element>
	</xsl:template>
	
	<xsl:template match="soudoby_korektor">
		<xsl:element name="soudoby_korektor">
			<xsl:apply-templates/>
		</xsl:element>
	</xsl:template>
	
<xsl:template match="torzo">
	<xsl:element name="torzo">
		<xsl:apply-templates />
	</xsl:element>
</xsl:template>
	
	<!-- obsah značky se převede do atributu -->
	<xsl:template match="bible_cislo_verse">
		<xsl:element name="vers">
			<xsl:attribute name="cislo">
				<xsl:value-of select="normalize-space(.)"/>
			</xsl:attribute>
			<!--<xsl:apply-templates />-->
		</xsl:element>
	</xsl:template>
	
	<!-- obsah značky se převede do atributu -->
	<xsl:template match="bible_cislo_kapitoly">
		<xsl:element name="kapitola">
			<xsl:attribute name="cislo">
				<xsl:choose>
					<xsl:when test="contains(., ',')">
						<xsl:value-of select="substring-before(normalize-space(.), ',')"/>
					</xsl:when>
					<xsl:otherwise>
						<xsl:value-of select="normalize-space(.)"/>
					</xsl:otherwise>
				</xsl:choose>
			</xsl:attribute>
			<!--<xsl:apply-templates />-->
		</xsl:element>
	</xsl:template>
	
	<!-- obsah značky se převede do atributu -->
	<xsl:template match="bible_zkratka_knihy">
		<xsl:element name="kniha">
			<xsl:attribute name="zkratka">
				<xsl:value-of select="normalize-space(.)"/>
			</xsl:attribute>
			<!--<xsl:apply-templates />-->
		</xsl:element>
	</xsl:template>
	
	
	<xsl:template match="delimitator_ekvivalentu">
		<xsl:element name="text">
			<xsl:apply-templates />
		</xsl:element>
	</xsl:template>
	
	<xsl:template match="kurziva">
		<xsl:element name="text">
			<xsl:apply-templates />
		</xsl:element>
	</xsl:template>
	
	
	
	<xsl:template match="lemma | hyperlemma">
		<xsl:element name="{name(.)}">
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
	
		<xsl:template match="textovy_orientator">
		<xsl:element name="textovy_orientator">
			<xsl:apply-templates />
		</xsl:element>
	</xsl:template>
	
	<xsl:template match="zive_zahlavi">
		<xsl:element name="zive_zahlavi">
			<xsl:apply-templates />
		</xsl:element>
	</xsl:template>
	
	<xd:doc scope="component">
		<xd:desc>
			<xd:p>Ignorované styly</xd:p>
		</xd:desc>
	</xd:doc>


<!-- Nebude se týkat Klareta -->
<!--	<xsl:template match="transliterace" />
	<xsl:template match="transliterace_cizi_jazyk" />
	<xsl:template match="transliterace_rozepsani_zkratky" />
	<xsl:template match="ruznocteni" />
	<xsl:template match="ruznocteni_autor" />
-->
	
	<xsl:template match="transliterace | transliterace_cizi_jazyk | transliterace_rozepsani_zkratky  | transliterace_horni_index">
		<xsl:if test="$exportovatTransliteraci = 'true()'">
		<xsl:element name="{name()}">
			<xsl:apply-templates />
		</xsl:element>
		</xsl:if>
	</xsl:template>
	
	<xsl:template match="transliterace/zkratka">
		<xsl:if test="$exportovatTransliteraci = 'true()'">
			<xsl:apply-templates />
		</xsl:if>
	</xsl:template>
	
	<xsl:template match="ruznocteni | ruznocteni_autor">
		<xsl:element name="{name()}">
			<xsl:apply-templates />
		</xsl:element>
	</xsl:template>
	

	<xsl:template match="Hlavicka"/>
	<xsl:template match="interni_poznamka"/>
	<xsl:template match="interni_poznamka_kurziva"/>
	<xsl:template match="interni_poznamka_horni_index" />
	<xsl:template match="interni_poznamka_preskrtnute" />
	<xsl:template match="pripisek_marginalni_mladsi" />
	<xsl:template match="pripisek_interlinearni_mladsi" />
	<xsl:template match="pripisek_interlinearni_mladsi_cizi_jazyk" />
	<xsl:template match="pripisek_marginalni_mladsi_cizi_jazyk" />
	<xsl:template match="poznamka" />
	<xsl:template match="poznamka_kurziva" />
	<xsl:template match="poznamka_horni_index" />
	<xsl:template match="Anotace" />
	<xsl:template match="Popisek_obrazku" />
	<xsl:template match="soubor" />
	<xsl:template match="Grantova_podpora" />
	<xsl:template match="Komercni_titul" />

	<xsl:template match="row" />
	<xsl:template match="cell" />
	
	<xsl:template match="relator" />
	<xsl:template match="znackovani" />
	

	<xsl:template match="Volny_radek" />

	<xsl:template match="podnadpis">
		<xsl:element name="{name()}">
			<xsl:apply-templates />
		</xsl:element>
	</xsl:template>

	<!-- dočasné -->
	<xsl:template match="kod" />
		<xsl:template match="pripisek" />
	<xsl:template match="Character_Style_2 | Character_Style_1 | Character_Style_3 | Character_Style_4 | Character_Style_5 | podnadpis_Char | Podnadpis_Char">
		<xsl:element name="text">
			<xsl:apply-templates />
		</xsl:element>
	</xsl:template>
	
	<xsl:template match="Body_Text | Body_Text_2 | Body_Text_Indent_2 | Plain_Text | Body_Text_3 | Body_Text_Indent | Body_Text_Indent_3 | Normal__Web_ | BodyTextIndent | BodyText">
		<xsl:element name="odstavec">
			<xsl:apply-templates />
		</xsl:element>
	</xsl:template>
	
	<xsl:template match="Nadpis_1 | Nadpis_2 | Nadpis_3 | Nadpis_4">
		<xsl:element name="nadpis">
			<xsl:apply-templates />
		</xsl:element>
	</xsl:template>
	
	<xsl:template match="Kurziva">
		<xsl:element name="text">
			<xsl:apply-templates />
		</xsl:element>
	</xsl:template>
	
	<xsl:template match="Edicni_komentar" />
	<xsl:template match="Edicni_komentar_Podnadpis" />
	<xsl:template match="Edicni_komentar_Nadpis" />
	
	<xsl:template match="poznamka_pod_carou" />
	<xsl:template match="footnote_reference" />

	<xsl:template match="pramen_preskrtnute" />

	<xsl:template match="horni_index">
		<xsl:element name="text">
			<xsl:apply-templates />
		</xsl:element>
	</xsl:template>
	
	<xsl:template match="rozepsani_zkratky">
			<xsl:apply-templates />
	</xsl:template>
	
	<xsl:template match="skrt" />
		
	
	<xsl:template match="rekonstrukce">
		<xsl:element name="rekonstrukce">
			<xsl:apply-templates />
		</xsl:element>
	</xsl:template>
	
	<xsl:template match="popisek_k_obrazku">
		<xsl:element name="obrazek">
			<xsl:apply-templates />
		</xsl:element>
	</xsl:template>
	
	<xsl:template match="transliterace/preskrtnute" />
	
	<xsl:template match="Adresat">
		<xsl:element name="adresat">
			<xsl:apply-templates />
		</xsl:element>
	</xsl:template>
	
	<xsl:template match="Impresum">
		<xsl:element name="impresum">
			<xsl:apply-templates />
		</xsl:element>
	</xsl:template>
	
	<xsl:template match="Marginalni_nadpis">
		<xsl:element name="nadpis">
			<xsl:apply-templates />
		</xsl:element>
	</xsl:template>
	
	
	<xsl:template match="marginalni_poznamka_cizi_jazyk">
		 <xsl:element name="cizi_jazyk">
		 	<xsl:apply-templates />
		 </xsl:element>
	</xsl:template>
	
	
	<xsl:template match="*">
		<xsl:message>
			<xsl:text>Prvek '</xsl:text>
			<xsl:value-of select="name(.)"/>
			<xsl:text>' nemá definvanou šablonu pro zpracování.</xsl:text>
		</xsl:message>
		<chyba>
			<prvek>
				<xsl:value-of select="name(.)"/>
			</prvek>
			<obsah>
				<xsl:value-of select="."/>
			</obsah>
		</chyba>
	</xsl:template>
	
</xsl:stylesheet>