<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" 
	xmlns:xd="http://www.oxygenxml.com/ns/doc/xsl" 
	xmlns:tei="http://www.tei-c.org/ns/1.0"
	xmlns="http://www.tei-c.org/ns/1.0"
	exclude-result-prefixes="xd tei" version="2.0">
	<xsl:import href="PohlGramSlov1756_Prevod-stylu.xsl"/>
	<xd:doc scope="stylesheet">
        <xd:desc>
            <xd:p><xd:b>Created on:</xd:b> Feb 5, 2013</xd:p>
            <xd:p><xd:b>Author:</xd:b> Boris</xd:p>
            <xd:p/>
        </xd:desc>
    </xd:doc>
	
	<!-- 
	<unclear><supplied>m</supplied>áme</unclear>
	-->
	

    <xsl:output method="xml" encoding="UTF-8" indent="yes"/>
    <xsl:strip-space elements="*"/>
	<xsl:preserve-space elements="metajazyk"/>
	<xsl:variable name="vychozi-jazyk" select="'cs'"/>

	<xsl:key name="poznamka-pod-carou" match="poznamka_pod_carou" use="@id"/>

<xsl:template match="body">
	<xsl:apply-templates />
</xsl:template>

<xsl:template match="/">
<xsl:text xml:space="preserve">
</xsl:text>
        <xsl:comment> PohlSlov1783_Prevod-Stylu </xsl:comment>
<xsl:text xml:space="preserve">
</xsl:text>
        	<body>
            	<xsl:apply-templates/>
        	</body>
    </xsl:template>


	<xsl:template match="Podnadpis/cestina"> 
		<choice>
			<xsl:apply-templates select="preceding-sibling::transkripce[1]" mode="transcription" />
			<orig xml:lang="cs-x-translit"><xsl:apply-templates /></orig>
		</choice>
	</xsl:template>
	
	<xsl:template match="Pismeno">
		<xsl:element name="head2">
			<xsl:apply-templates/>
		</xsl:element>
	</xsl:template>
	
	<xsl:template match="Pismeno/nemcina">
		<foreign xml:lang="de">
			<xsl:apply-templates/>
		</foreign>
	</xsl:template>
	
	<!-- delimitátor ekvivalentů (tabulátor) -->
	<xsl:template match="metajazyk[. = '&#9;']" />
	
	<xsl:template match="metajazyk">
		<xsl:choose>
			<xsl:when test="preceding-sibling::*[1]/self::cestina">
				
			</xsl:when>
			<xsl:when test="following-sibling::*[1]/self::cestina">
				
			</xsl:when>
			<xsl:when test="following-sibling::*[1]/self::delimitator">
				
			</xsl:when>
			<xsl:when test="following-sibling::*[1]/self::doplneny_text">
				
			</xsl:when>
			<xsl:when test="following-sibling::*[1]/self::transkripce">
				
			</xsl:when>
			<xsl:otherwise>
				<label xml:lang="de"><xsl:apply-templates /></label>				
			</xsl:otherwise>
		</xsl:choose>
		
	</xsl:template>
	
	<xsl:template match="metajazyk[. = '&#9;']" mode="transliteration" />
	<xsl:template match="metajazyk" mode="transliteration">
		<seg><label xml:lang="de"><xsl:apply-templates /></label></seg>
	</xsl:template>

	<xsl:template match="cestina_vytceny_vyraz" />
<!--	<xsl:template match="cestina_vytceny_vyraz">
		<orig xml:lang="cs-x-translit">
		<supplied>
				<xsl:apply-templates />
		</supplied>
		</orig>
	</xsl:template>-->
	
	<xsl:template match="cestina_vytceny_vyraz" mode="transliteration">
		<orig xml:lang="cs-x-translit">
			<supplied><xsl:apply-templates /></supplied>
		</orig>
	</xsl:template>
	
	<xsl:template match="nemcina_vytceny_vyraz">
		<cit type="translation" xml:lang="de">
			<quote><supplied><xsl:apply-templates /></supplied></quote>
		</cit>
	</xsl:template>
	
	<xsl:template match="doplneny_text_nemcina">
		<cit type="translation" xml:lang="de">
			<quote><unclear><supplied><xsl:apply-templates /></supplied></unclear></quote>
		</cit>
	</xsl:template>
	
	<xsl:template match="Heslova_stat/transkripce" priority="5">
		<xsl:element name="form">
			<xsl:attribute name="xml:id">
				<xsl:value-of select="concat('en', substring(string(1000001 + count(parent::*/preceding-sibling::Heslova_stat)), 2), '.hw1')"/>
			</xsl:attribute>
			<orth xml:lang="cs" >
				<choice>
					<reg xml:lang="cs-x-transcr"><xsl:apply-templates select="text()" /></reg>
					<xsl:choose>
						<xsl:when test="count(following-sibling::nemcina[1]/preceding-sibling::node()) &lt; count(following-sibling::nemcina_vytceny_vyraz[1]/preceding-sibling::node())">
							<xsl:apply-templates select="following-sibling::nemcina[1]/preceding-sibling::* except self::* except preceding-sibling::*" mode="transliteration" />
						</xsl:when>
						<xsl:when test="count(following-sibling::nemcina_vytceny_vyraz[1]/preceding-sibling::node()) &gt; 0 and count(following-sibling::nemcina_vytceny_vyraz[1]/preceding-sibling::node()) &lt; count(following-sibling::nemcina[1]/preceding-sibling::node())">
							<xsl:apply-templates select="following-sibling::nemcina_vytceny_vyraz[1]/preceding-sibling::* except self::* except preceding-sibling::*" mode="transliteration" />
						</xsl:when>
						<xsl:when test="preceding-sibling::*[1]/self::nemcina">
							<xsl:apply-templates select="following-sibling::*" mode="transliteration" />
						</xsl:when>
						<xsl:otherwise>
							<xsl:apply-templates select="following-sibling::nemcina[1]/preceding-sibling::* except self::* except preceding-sibling::*" mode="transliteration" />
						</xsl:otherwise>
					</xsl:choose>
				</choice>
			</orth>
		</xsl:element>
	</xsl:template>
	
	<xsl:template match="Heslova_stat/transkripce" mode="transliteration">
		<reg xml:lang="cs-x-transcr"><xsl:apply-templates select="text()" /></reg>
	</xsl:template>
	
	<xsl:template match="Heslova_stat/transkripce[position() &gt; 1]" priority="10" />
	
	<xsl:template match="doplneny_text" mode="transliteration">
		<orig xml:lang="cs-x-translit">
			<unclear><supplied><xsl:apply-templates /></supplied></unclear>
		</orig>
	</xsl:template>
	
	<xsl:template match="doplneny_text" />
	
</xsl:stylesheet>
