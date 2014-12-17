<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet 
	xmlns:xsl="http://www.w3.org/1999/XSL/Transform" 
	xmlns:xd="http://www.oxygenxml.com/ns/doc/xsl"
	xmlns:xml="http://www.w3.org/XML/1998/namespace"
	exclude-result-prefixes="xd" version="1.0">
	<xd:doc scope="stylesheet">
		<xd:desc>
			<xd:p><xd:b>Created on:</xd:b> Dec 3, 2010</xd:p>
			<xd:p><xd:b>Author:</xd:b> Boris Lehečka</xd:p>
			<xd:p>Převodník společných stylů, využitelných v obou výstupech</xd:p>
		</xd:desc>
	</xd:doc>

	<xsl:include href="TEI_Foliace_na_Pb.xsl"/>
	
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
<!--
	<xsl:template match="text">
		<xsl:apply-templates/>
	</xsl:template>
-->

<xsl:template match="body">
	<xsl:element name="body">
		<xsl:attribute name="xml" namespace="http://www.w3.org/XML/1998/namespace"><xsl:text>http://www.w3.org/XML/1998/namespace</xsl:text></xsl:attribute>
		<xsl:apply-templates />
	</xsl:element>
</xsl:template>
	
	<xsl:template match="text">
		<text>
			<xsl:apply-templates/>
		</text>
	</xsl:template>
	
	
	<xsl:template match="Titul">
		<xsl:comment> Titul </xsl:comment>
		<xsl:element name="thead">
			<xsl:apply-templates/>
		</xsl:element>
	</xsl:template>
	
	<xsl:template match="Nadpis | Bible_Nadpis_kapitoly">
		<xsl:comment> Nadpis </xsl:comment>
		<xsl:element name="head">
			<xsl:apply-templates/>
		</xsl:element>
	</xsl:template>


	<xsl:template match="Podnadpis">
		<xsl:comment> Podnadpis </xsl:comment>
		<xsl:element name="head1">
			<xsl:apply-templates/>
		</xsl:element>
	</xsl:template>


	<xsl:template match="Vers">
		<xsl:element name="l">
			<xsl:attribute name="id" namespace="http://www.w3.org/XML/1998/namespace">
				<xsl:value-of select="concat(generate-id(), '.', position())"/>
			</xsl:attribute>
			<xsl:apply-templates/>
		</xsl:element>
	</xsl:template>

	<xsl:template match="Volny_radek">
		<xsl:element name="p">
			<xsl:attribute name="rend"><xsl:text>vspace</xsl:text></xsl:attribute>
			<xsl:attribute name="id" namespace="http://www.w3.org/XML/1998/namespace">
				<xsl:value-of select="concat(generate-id(), '.', position())"/>
			</xsl:attribute>
		</xsl:element>
	</xsl:template>
	
	<xsl:template match="Normalni">
		<xsl:element name="p">
			<xsl:attribute name="id" namespace="http://www.w3.org/XML/1998/namespace">
				<xsl:value-of select="generate-id()"/>
			</xsl:attribute>
			<xsl:apply-templates/>
		</xsl:element>
	</xsl:template>

	<xsl:template match="Doprava">
		<xsl:element name="p">
			<xsl:attribute name="rend">right</xsl:attribute>
			<xsl:attribute name="id" namespace="http://www.w3.org/XML/1998/namespace">
				<xsl:value-of select="generate-id()"/>
			</xsl:attribute>
			
			<xsl:apply-templates/>
		</xsl:element>
	</xsl:template>

<!--
	<xsl:template match="foliace">
		<xsl:variable name="obsah">
			<xsl:value-of select="text()"/>
		</xsl:variable>
		<xsl:variable name="mezera">
			<xsl:value-of select="contains(substring($obsah, string-length($obsah)), ' ')"/>
		</xsl:variable>

	<xsl:choose>
		<xsl:when test="contains($obsah, ' ')">
			<xsl:call-template name="vlozitPb">
				<xsl:with-param name="obsah" select="substring-before($obsah, ' ')" />
				<xsl:with-param name="mezera" select="$mezera" />
			</xsl:call-template>
		</xsl:when>
		<xsl:otherwise>
			<xsl:call-template name="vlozitPb">
				<xsl:with-param name="obsah" select="$obsah" />
			</xsl:call-template>
		</xsl:otherwise>
	</xsl:choose>
		
		
<!-\-		<xsl:element name="pb">
			<xsl:attribute name="n">
				<xsl:value-of select="."/>
			</xsl:attribute>
		</xsl:element>
-\->
	</xsl:template>
	
	<xsl:template name="vlozitPb">
		<xsl:param name="obsah" />
		<xsl:param name="mezera" />
		<xsl:element name="pb">
		<xsl:choose>
			<xsl:when test="contains($obsah, 'ed.')">
				<xsl:attribute name="type"><xsl:text>edition</xsl:text></xsl:attribute>
				<xsl:attribute name="n"><xsl:value-of select="substring-before($obsah, 'ed.')"/></xsl:attribute>
			</xsl:when>
			<xsl:when test="contains($obsah, 'st.')">
				<xsl:attribute name="type"><xsl:text>print</xsl:text></xsl:attribute>
				<xsl:attribute name="n"><xsl:value-of select="substring-before($obsah, 'st.')"/></xsl:attribute>
			</xsl:when>
			<xsl:otherwise>
				<xsl:attribute name="n"><xsl:value-of select="$obsah"/></xsl:attribute>
			</xsl:otherwise>
		</xsl:choose>
			<xsl:if test="$mezera">
				<xsl:attribute name="rend"><xsl:text>space</xsl:text></xsl:attribute>
			</xsl:if>
		</xsl:element>
		<xsl:if test="contains($obsah, ' ')">
			<xsl:call-template name="vlozitPb">
				<xsl:with-param name="obsah" select="substring-after($obsah, ' ')" />
				<xsl:with-param name="mezera" select="$mezera" />
			</xsl:call-template>
		</xsl:if>
	</xsl:template>
	-->
	<xsl:template match="cizi_jazyk">
		<xsl:element name="foreign">
			<xsl:apply-templates />
		</xsl:element>
	</xsl:template>

	<xsl:template match="cislo_verse">
		<xsl:element name="lb">
			<xsl:attribute name="n">
				<xsl:value-of select="."/>
			</xsl:attribute>
		</xsl:element>
	</xsl:template>

	<xsl:template match="pramen">
		<xsl:element name="sic">
			<xsl:value-of select="normalize-space(.)"/>
			<!--<xsl:apply-templates />-->
		</xsl:element>
	</xsl:template>

	<xsl:template match="pramen_horni_index">
		<xsl:element name="sic">
			<xsl:element name="hi">
				<xsl:attribute name="rend">
					<xsl:text>superscript</xsl:text>
				</xsl:attribute>
				<xsl:value-of select="normalize-space(.)"/>
				<!--<xsl:apply-templates />-->
			</xsl:element>
		</xsl:element>
	</xsl:template>



	<xsl:template match="emendace">
		<xsl:element name="corr">
			<xsl:value-of select="normalize-space(.)"/>
			<!--<xsl:apply-templates />-->
		</xsl:element>
	</xsl:template>

	<xsl:template match="doplneny_text">
		<xsl:element name="supplied">
			<xsl:apply-templates/>
		</xsl:element>
	</xsl:template>

	<xsl:template match="poznamka">
		<xsl:element name="note">
			<xsl:apply-templates/>
		</xsl:element>
	</xsl:template>

	<xsl:template match="poznamka_kurziva">
		<xsl:element name="note">
			<xsl:element name="hi">
				<xsl:attribute name="rend">
					<xsl:text>italic</xsl:text>
				</xsl:attribute>
				<xsl:apply-templates/>
			</xsl:element>
		</xsl:element>
	</xsl:template>

	<xsl:template match="poznamka_horni_index">
		<xsl:element name="note">
			<xsl:element name="hi">
				<xsl:attribute name="rend">
					<xsl:text>sup</xsl:text>
				</xsl:attribute>
				<xsl:apply-templates/>
			</xsl:element>
		</xsl:element>
	</xsl:template>
	
	<xsl:template match="pripisek_marginalni_mladsi">
		<xsl:element name="add">
			<xsl:attribute name="place">
				<xsl:text>margin</xsl:text>
			</xsl:attribute>
			<xsl:attribute name="type">
				<xsl:text>non-contemporaneous</xsl:text>
			</xsl:attribute>
			<xsl:apply-templates/>
		</xsl:element>
	</xsl:template>

	<xsl:template match="pripisek_interlinearni_mladsi">
		<xsl:element name="add">
			<xsl:attribute name="place">
				<xsl:text>inline</xsl:text>
			</xsl:attribute>
			<xsl:attribute name="type">
				<xsl:text>non-contemporaneous</xsl:text>
			</xsl:attribute>
			<xsl:apply-templates/>
		</xsl:element>
	</xsl:template>



	<xsl:template match="pripisek_marginalni_soudoby">
		<xsl:element name="add">
			<xsl:attribute name="place">
				<xsl:text>margin</xsl:text>
			</xsl:attribute>
			<xsl:attribute name="type">
				<xsl:text>contemporaneous</xsl:text>
			</xsl:attribute>
			<xsl:apply-templates/>
		</xsl:element>
	</xsl:template>
	
	<xsl:template match="pripisek_interlinearni_soudoby">
		<xsl:element name="add">
			<xsl:attribute name="place">
				<xsl:text>inline</xsl:text>
			</xsl:attribute>
			<xsl:attribute name="type">
				<xsl:text>contemporaneous</xsl:text>
			</xsl:attribute>
			<xsl:apply-templates/>
		</xsl:element>
	</xsl:template>
	
	<xsl:template match="pripisek_marginalni_mladsi_cizi_jazyk">
		<xsl:element name="add">
			<xsl:attribute name="place">
				<xsl:text>margin</xsl:text>
			</xsl:attribute>
			<xsl:attribute name="type">
				<xsl:text>non-contemporaneous</xsl:text>
			</xsl:attribute>
			<xsl:element name="foreign">
				<xsl:apply-templates />
			</xsl:element>
		</xsl:element>
	</xsl:template>
	
	<xsl:template match="pripisek_interlinearni_mladsi_cizi_jazyk">
		<xsl:element name="add">
			<xsl:attribute name="place">
				<xsl:text>inline</xsl:text>
			</xsl:attribute>
			<xsl:attribute name="type">
				<xsl:text>non-contemporaneous</xsl:text>
			</xsl:attribute>
			<xsl:element name="foreign">
				<xsl:apply-templates />
			</xsl:element>
		</xsl:element>
	</xsl:template>

	<xsl:template match="table">
		<xsl:element name="table">
			<xsl:copy-of select="@*"/>
			<xsl:apply-templates />
		</xsl:element>
	</xsl:template>

	<xsl:template match="row">
		<xsl:element name="row">
			<xsl:copy-of select="@*"/>
			<xsl:apply-templates />
		</xsl:element>
	</xsl:template>

	<xsl:template match="cell">
		<xsl:element name="cell">
			<xsl:copy-of select="@*"/>
			<xsl:if test="string-length(.) = 0">
				<xsl:text>&#xA0;</xsl:text>
			</xsl:if>
			<xsl:apply-templates />
		</xsl:element>
	</xsl:template>
	
	
	<xsl:template match="druhy_preklad">
		<xsl:element name="add">
			<xsl:attribute name="place"><xsl:text>inline</xsl:text></xsl:attribute>
			<xsl:attribute name="type"><xsl:text>contemporaneous</xsl:text></xsl:attribute>
			<xsl:attribute name="hand"><xsl:text>#XY</xsl:text></xsl:attribute>
			<xsl:apply-templates />
		</xsl:element>
	</xsl:template>
	
	<xsl:template match="soudoby_korektor">
		<xsl:element name="add">
			<xsl:attribute name="place"><xsl:text>inline</xsl:text></xsl:attribute>
			<xsl:attribute name="type"><xsl:text>contemporaneous</xsl:text></xsl:attribute>
			<xsl:attribute name="hand"><xsl:text>#XY</xsl:text></xsl:attribute>
			<xsl:apply-templates />
		</xsl:element>
	</xsl:template>
	
	<xsl:template match="relator">
		<xsl:element name="anchor">
			<xsl:attribute name="type">
				<xsl:choose>
					<xsl:when test="text() = '{'">
						<xsl:text>start</xsl:text>
					</xsl:when>
					<xsl:otherwise>
						<xsl:text>end</xsl:text>
					</xsl:otherwise>
				</xsl:choose>
			</xsl:attribute>
			<xsl:attribute name="xml:id">
				<xsl:text>ra.</xsl:text><xsl:value-of select="generate-id()"/><xsl:text>.</xsl:text><xsl:value-of select="position()"/>
			</xsl:attribute>
		</xsl:element>
	</xsl:template>
	
	<xsl:template match="zive_zahlavi">
		<xsl:element name="fw">
			<xsl:apply-templates />
		</xsl:element>
	</xsl:template>

	<xd:doc scope="component">
		<xd:desc>
			<xd:p>Ignorované styly</xd:p>
		</xd:desc>
	</xd:doc>


	<xsl:template match="Hlavicka"/>
	<xsl:template match="delimitator_ekvivalentu"/>
	<xsl:template match="interni_poznamka"/>
	<xsl:template match="interni_poznamka_horni_index"/>
	<xsl:template match="interni_poznamka_kurziva"/>
	<xsl:template match="hyperlemma"/>
	<xsl:template match="lemma"/>
	<xsl:template match="pripisek"/>
	<xsl:template match="ruznocteni"/>
	<xsl:template match="ruznocteni_autor"/>
	<xsl:template match="poznamka_pod_carou"/>
	
</xsl:stylesheet>
