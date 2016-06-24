<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" 
	xmlns:xd="http://www.oxygenxml.com/ns/doc/xsl" 
	xmlns:tei="http://www.tei-c.org/ns/1.0"
	xmlns="http://www.tei-c.org/ns/1.0"
	exclude-result-prefixes="xd tei" version="2.0">
	<xsl:import href="COMMON_Unknown_element.xsl" />
	<xsl:import href="Pagina.xsl"/>
	
	<xd:doc scope="stylesheet">
        <xd:desc>
            <xd:p><xd:b>Created on:</xd:b> Feb 5, 2013</xd:p>
            <xd:p><xd:b>Author:</xd:b> Boris</xd:p>
            <xd:p/>
        </xd:desc>
    </xd:doc>

    <xsl:output method="xml" encoding="UTF-8" indent="yes"/>
    <xsl:strip-space elements="*"/>
	<xsl:preserve-space elements="text"/>
	<xsl:variable name="vychozi-jazyk" select="'cs'"/>

	<xsl:template match="body">
		<xsl:apply-templates />
	</xsl:template>

	<xsl:template match="/">
		<xsl:text xml:space="preserve">
		</xsl:text>
        <xsl:comment> DDBW_Rename </xsl:comment>
		<xsl:text xml:space="preserve">
		</xsl:text>
        <body>
            <xsl:apply-templates/>
        </body>
    </xsl:template>

	<xsl:template match="Predmluva">
		<p>
			<xsl:apply-templates/>
		</p>
	</xsl:template>
	
	<xsl:template match="Predmluva/Nemcina">
		<foreign xml:lang="de-x-transcr">
			<xsl:apply-templates/>
		</foreign>
	</xsl:template>
	
	<xsl:template match="Titul">
		<xsl:element name="head0">
			<xsl:if test="node()[self::Nemcina]">
				<xsl:attribute name="xml:lang">
					<xsl:text>de-x-translit</xsl:text>
				</xsl:attribute>
			</xsl:if>
			
			<xsl:apply-templates />
			<xsl:if test="following-sibling::*[1]/self::Titul">
				<lb />
				<xsl:apply-templates select="following-sibling::Titul[1]" mode="following" />
			</xsl:if>
		</xsl:element>
	</xsl:template>
	
	<xsl:template match="Titul[preceding-sibling::*[1]/self::Titul]" />
	
	<xsl:template match="Titul/Nemcina">
		<xsl:apply-templates/>
	</xsl:template>
	
	<xsl:template match="Titul" mode="following">
		<xsl:apply-templates />
	</xsl:template>
	
	<xsl:template match="Litera">
		<xsl:element name="head1">
			<xsl:if test="node()[self::Nemcina]">
				<xsl:attribute name="xml:lang">
					<xsl:text>de-x-translit</xsl:text>
				</xsl:attribute>
			</xsl:if>
			
			<xsl:apply-templates/>
		</xsl:element>
	</xsl:template>
	
	<xsl:template match="Litera/Nemcina">
		<xsl:apply-templates/>
	</xsl:template>
  
  <xsl:template match="Titul/text">
		<xsl:apply-templates/>
	</xsl:template>
  
  <xsl:template match="Komentar">
    <note><xsl:apply-templates/></note>
	</xsl:template>

  <xsl:template match="Sloupec">
    <xsl:element name="cb">
      <xsl:attribute name="n">
        <xsl:value-of select="text()"/>
      </xsl:attribute>
    </xsl:element>
  </xsl:template>
  
  <xsl:template match="Zaver">
    <xsl:element name="p">
      <xsl:attribute name="rend">
        <xsl:text>zaver</xsl:text>
      </xsl:attribute>
      
		  <xsl:apply-templates/>
    </xsl:element>
	</xsl:template>
  
  <xsl:template match="Zaver/Nemcina">
    <foreign xml:lang="de-x-transcr">
      <xsl:apply-templates/>
    </foreign>
	</xsl:template>
	
	<xsl:template match="Heslovy_Odstavec">
		<xsl:element name="entryFree">
			<xsl:for-each-group select="node()" group-ending-with="Cislo_Vyznamu">
				<xsl:if test="position() = 1">
					<xsl:apply-templates select="current-group()[self::Stranka]"/>
					
					<sense>
						<xsl:apply-templates select="current-group()[not (self::Cislo_Vyznamu)][not (self::Stranka)]" mode="transliteration"/>
					</sense>
				</xsl:if>
			</xsl:for-each-group>
			
			<xsl:for-each-group select="node()" group-starting-with="Cislo_Vyznamu">
				<xsl:if test="position() > 1">
					<sense>
						<xsl:apply-templates select="current-group()" mode="transliteration"/>
					</sense>
				</xsl:if>
			</xsl:for-each-group>
		</xsl:element>
	</xsl:template>
	
	<xsl:template match="Nemcina" mode="transliteration">
		<form>
			<orth xml:lang="de-x-translit">
				<xsl:apply-templates />
			</orth>
		</form>
	</xsl:template>
	
	<!--
	<xsl:template match="Heslovy_Odstavec/Nemcina">
		<sense>
			<xsl:apply-templates select="following-sibling::Nemcina[1]/preceding-sibling::Cislo_Vyznamu except self::* except preceding-sibling::*" mode="sense_num" />
			<xsl:element name="form">
				<orth xml:lang="de" >
					<choice>
						<reg xml:lang="cs-x-transcr"><xsl:apply-templates select="text()" /></reg>
						<xsl:apply-templates select="following-sibling::Nemcina[1]/preceding-sibling::* except self::* except preceding-sibling::*" mode="transliteration" />
					</choice>
				</orth>
			</xsl:element>
		</sense>
	</xsl:template>
	-->
	
	<xsl:template match="Cislo_Vyznamu" mode="transliteration">
		<num><xsl:apply-templates /></num>
	</xsl:template>
	<xsl:template match="Kvalifikator" mode="transliteration">
		<iType>
			<xsl:apply-templates />
		</iType>
	</xsl:template>
	<xsl:template match="Cestina" mode="transliteration">
		<cit type="translation" xml:lang="cs"><quote xml:lang="cs">
      <choice>
        <reg xml:lang="cs-x-transc" xml:compute-reg="true"><xsl:apply-templates mode="strip-node-with-values" /></reg>
        <orig xml:lang="cs-x-translit" xml:space="preserve"><xsl:apply-templates /></orig>
      </choice> 
    </quote></cit>
	</xsl:template>
	<xsl:template match="Latina" mode="transliteration">
		<cit type="translation" xml:lang="lat-x-translit"><quote><xsl:apply-templates /></quote></cit>
	</xsl:template>

  <xsl:template match="*" mode="strip-node-with-values" />
	
	<xsl:template match="Pododstavec">
		<xsl:element name="entryFree">
			<xsl:attribute name="type">
				<xsl:text>subentry</xsl:text>
			</xsl:attribute>
			
			<xsl:apply-templates mode="transliteration" />
		</xsl:element>
	</xsl:template>
	
	<xsl:template match="Pododstavec/Sloupec" mode="transliteration">
		<xsl:element name="cb">
			<xsl:attribute name="n">
				<xsl:value-of select="text()"/>
			</xsl:attribute>
		</xsl:element>
	</xsl:template>
	
	<xsl:template match="Pododstavec/Nemcina" mode="transliteration">
		<form>
			<orth xml:lang="de-x-translit"><xsl:apply-templates /></orth>
		</form>
	</xsl:template>
</xsl:stylesheet>
