<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
	xmlns:xd="http://www.oxygenxml.com/ns/doc/xsl"
	exclude-result-prefixes="xd"
	version="1.0">
	<xd:doc scope="stylesheet">
		<xd:desc>
			<xd:p><xd:b>Created on:</xd:b> Jan 9, 2011</xd:p>
			<xd:p><xd:b>Author:</xd:b> boris</xd:p>
			<xd:p>Rozepíše kumulativní styly (např. <xd:b>poznamka_kurziva</xd:b>  na více jednochých stylů.</xd:p>
		</xd:desc>
	</xd:doc>
	
	<xsl:include href="Kopirovani_prvku.xsl"/>
		
	<xsl:template match="/">
		<xsl:comment> Prevest_sloucene_styly_na_elementy </xsl:comment>
		<xsl:apply-templates />
	</xsl:template>
	
	<xsl:template match="poznamka_kurziva">
			<xsl:element name="poznamka">
				<xsl:element name="kurziva">
					<xsl:value-of select="."/>
				</xsl:element>
			</xsl:element>
		</xsl:template>
	
	<xsl:template match="poznamka_preskrtnute">
		<xsl:element name="poznamka">
			<xsl:element name="preskrtnute">
				<xsl:value-of select="."/>
			</xsl:element>
		</xsl:element>
	</xsl:template>
	
	<xsl:template match="poznamka_horni_index">
		<xsl:element name="poznamka">
			<xsl:element name="horni_index">
				<xsl:value-of select="."/>
			</xsl:element>
		</xsl:element>
	</xsl:template>
	
	<xsl:template match="interni_poznamka_dolni_index">
		<xsl:element name="interni_poznamka">
				<xsl:element name="dolni_index">
					<xsl:value-of select="."/>
				</xsl:element>
		</xsl:element>
	</xsl:template>
	
	<xsl:template match="transliterace_rozepsani_zkratky">
		<xsl:element name="transliterace">
			<xsl:element name="rozepsani_zkratky">
				<xsl:value-of select="."/>
			</xsl:element>
		</xsl:element>
	</xsl:template>
	
	<xsl:template match="transliterace_horni_index">
		<xsl:element name="transliterace">
			<xsl:element name="horni_index">
				<xsl:value-of select="."/>
			</xsl:element>
		</xsl:element>
	</xsl:template>
	<xsl:template match="transliterace_cizi_jazyk">
		<xsl:element name="transliterace">
			<xsl:element name="cizi_jazyk">
				<xsl:value-of select="."/>
			</xsl:element>
		</xsl:element>
	</xsl:template>
	<xsl:template match="transliterace_zkratka">
		<xsl:element name="transliterace">
			<xsl:element name="zkratka">
				<xsl:value-of select="."/>
			</xsl:element>
		</xsl:element>
	</xsl:template>
	<xsl:template match="transliterace_zkratka_horni_index">
		<xsl:element name="transliterace">
			<xsl:element name="zkratka">
				<xsl:element name="horni_index">
				<xsl:value-of select="."/>
				</xsl:element>
			</xsl:element>
		</xsl:element>
	</xsl:template>
	<xsl:template match="transliterace_preskrtnute">
		<xsl:element name="transliterace">
			<xsl:element name="preskrtnute">
				<xsl:value-of select="."/>
			</xsl:element>
		</xsl:element>
	</xsl:template>
	<xsl:template match="transliterace_cizi_jazyk_horni_index">
		<xsl:element name="transliterace">
			<xsl:element name="cizi_jazyk">
				<xsl:element name="horni_index">
					<xsl:value-of select="."/>
				</xsl:element>
			</xsl:element>
		</xsl:element>
	</xsl:template>
	
	<xsl:template match="rekonstrukce_cizi_jazyk">
		<xsl:element name="cizi_jazyk">
			<xsl:element name="rekonstrukce">
				<xsl:value-of select="."/>
			</xsl:element>
		</xsl:element>
	</xsl:template>
	
	<xsl:template match="pripisek_marginalni_mladsi_torzo">
		<xsl:element name="pripisek_marginalni_mladsi">
			<xsl:element name="torzo">
				<xsl:value-of select="."/>
			</xsl:element>
		</xsl:element>
	</xsl:template>
	
	<xsl:template match="pripisek_marginalni_mladsi_doplneny_text">
		<xsl:element name="pripisek_marginalni_mladsi">
			<xsl:element name="doplneny_text">
				<xsl:value-of select="."/>
			</xsl:element>
		</xsl:element>
	</xsl:template>
	
	<xsl:template match="popisek_k_obrazku_rekonstrukce">
		<xsl:element name="popisek_k_obrazku">
			<xsl:element name="rekonstrukce">
				<xsl:value-of select="."/>
			</xsl:element>
		</xsl:element>
	</xsl:template>
	
	<xsl:template match="popisek_k_obrazku_doplneny_text">
		<xsl:element name="popisek_k_obrazku">
			<xsl:element name="doplneny_text">
				<xsl:value-of select="."/>
			</xsl:element>
		</xsl:element>
	</xsl:template>
	
	<xsl:template match="popisek_k_obrazku_cizi_jazyk">
		<xsl:element name="popisek_k_obrazku">
			<xsl:element name="cizi_jazyk">
				<xsl:value-of select="."/>
			</xsl:element>
		</xsl:element>
	</xsl:template>
	
	<xd:doc>
		<xd:desc></xd:desc>
	</xd:doc>
	<xsl:template match="popisek_k_obrazku_torzo">
		<xsl:element name="popisek_k_obrazku">
			<xsl:element name="torzo">
				<xsl:value-of select="."/>
			</xsl:element>
		</xsl:element>
	</xsl:template>
</xsl:stylesheet>