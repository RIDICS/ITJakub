<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" 
	xmlns:xd="http://www.oxygenxml.com/ns/doc/xsl"
	xmlns:xml="http://www.w3.org/XML/1998/namespace"
	xmlns:func="http://exslt.org/functions"
	xmlns:vwf="http://vokabular.ujc.cas.cz/xslt/functions"
	extension-element-prefixes="func"
	exclude-result-prefixes="xd vwf" version="1.0"
	>
	<xd:doc scope="stylesheet">
		<xd:desc>
			<xd:p><xd:b>Created on:</xd:b> Dec 3, 2010</xd:p>
			<xd:p><xd:b>Author:</xd:b> boris</xd:p>
			<xd:p></xd:p>
		</xd:desc>
	</xd:doc>

	<xsl:include href="Kopirovani_prvku.xsl"/>
	
	<xsl:output omit-xml-declaration="no" indent="yes"/>
	<xsl:strip-space elements="*"/>

	<xsl:variable name="interpunkce" select="'?.,;!:„“‚‘’#'"/>
	<xsl:variable name="mezera" select="' '" />
	<xsl:variable name="interpunkcePlusMezera" select="concat($interpunkce, $mezera)"/>
	
	<xsl:template match="/">
		<xsl:comment> EM_Upravit_obsah_prvku_unclear </xsl:comment>
		<xsl:apply-templates/>
	</xsl:template>


	<xsl:template match="text[following-sibling::*[1]/self::unclear and preceding-sibling::*[1]/self::unclear]" priority="10">
		<xsl:copy>
<!--			<xsl:attribute name="xml:space"><xsl:text>preserve</xsl:text></xsl:attribute>-->
			<xsl:copy-of select="@*"/>
			<xsl:choose>
				<!-- předchozí unclear nekončí mezerou, samotný text nezačíná mezerou ani interpunkcí a obsahuje mezeru  -->
				<xsl:when test="substring(preceding-sibling::*[1], string-length(preceding-sibling::*[1]), 1) != ' ' and contains(., $mezera) and translate(substring(., 1, 1), $interpunkcePlusMezera, '') != ''">
					
					<xsl:variable name="znak">
						<xsl:call-template name="najitPrvniVyskytujiciSeZnak">
							<xsl:with-param name="text" select="." />
							<xsl:with-param name="interpunkce" select="$interpunkcePlusMezera" />
						</xsl:call-template>
					</xsl:variable>
					<xsl:call-template name="vlozit-text-pred-interpunkci">
						<xsl:with-param name="text" select="."/>
						<xsl:with-param name="hledanyZnak" select="$znak" />
					</xsl:call-template>
					<xsl:value-of select="$znak"/>
					<!--<xsl:call-template name="substring-after-including">
						<xsl:with-param name="text" select="." />
						<xsl:with-param name="znak" select="$znak" />
					</xsl:call-template>-->
					
					<!--
						<xsl:call-template name="vlozit-text-pred-interpunkci">
						<xsl:with-param name="text" select="."/>
					</xsl:call-template>-->
					
					<xsl:call-template name="substring-before-last">
						<xsl:with-param name="delimiter" select="$mezera" />
						<xsl:with-param name="string" select="substring-after(., $znak)" />
					</xsl:call-template>
					<xsl:value-of select="$mezera"/>
					
					
	<!--				<xsl:choose>
						<xsl:when test="translate(substring(., string-length(.), 1), $interpunkcePlusMezera, '') != ''">
							<xsl:call-template name="substring-before-last">
								<xsl:with-param name="delimiter" select="$mezera" />
								<!-\-<xsl:with-param name="string" select="substring-after(., $mezera)" />-\->
								<xsl:with-param name="string" select="substring-after(., $znak)" />
							</xsl:call-template>
							<xsl:value-of select="$mezera"/>
						</xsl:when>
						<xsl:otherwise>
							<xsl:value-of select="substring-after(., $mezera)"/>
						</xsl:otherwise>
					</xsl:choose>-->

				</xsl:when>
				<xsl:otherwise>
					<xsl:choose>
						<!-- unclear nezačíná mezerou -->
						<xsl:when test="substring(preceding-sibling::*[1], string-length(preceding-sibling::*[1]), 1) != ' ' and contains(., $mezera) and translate(substring(., 1, 1), $interpunkcePlusMezera, '') != ''">
							<xsl:value-of select="$mezera"/>
							<xsl:value-of select="substring-after(., $mezera)"/>
						</xsl:when>
						<xsl:otherwise>
							<xsl:apply-templates />
						</xsl:otherwise>
					</xsl:choose>
				</xsl:otherwise>
			</xsl:choose>
		
		</xsl:copy>
	</xsl:template>
	<xsl:template name="vlozit-text-pred-interpunkci">
		<xsl:param name="text" />
		<xsl:param name="hledanyZnak" select="''" />
		<xsl:variable name="znak">
			
			<xsl:choose>
				<xsl:when test="$hledanyZnak != ''">
					<xsl:value-of select="$hledanyZnak"/>
				</xsl:when>
				<xsl:otherwise>
					<xsl:call-template name="najitPrvniVyskytujiciSeZnak">
						<xsl:with-param name="text" select="$text" />
						<xsl:with-param name="interpunkce" select="$interpunkcePlusMezera" />
					</xsl:call-template>
				</xsl:otherwise>
			</xsl:choose>
			
		</xsl:variable>

		<xsl:choose>
			<xsl:when test="$znak = ''" />
			<xsl:otherwise>
				<!--<xsl:value-of select="substring-before(substring($text, string-length(substring-before($text, $znak)) + 1), $mezera)" />-->
				<xsl:value-of select="substring-before(substring($text, string-length(substring-before($text, $znak)) + 1), $znak)" />
			</xsl:otherwise>
		</xsl:choose>

<!--		<xsl:value-of select="$mezera" />-->
		
	</xsl:template>

	<xsl:template match="text[following-sibling::*[1]/self::unclear]">
		<xsl:copy>
			<xsl:copy-of select="@*"/>
			<xsl:choose>
				<!-- unclear začíná na interpunkci -->
				<xsl:when test="translate(substring(following-sibling::unclear, 1, 1), $interpunkce, '') = ''">
					<xsl:apply-templates />
				</xsl:when>
				<xsl:when test="translate(substring(., string-length(.), 1), $interpunkcePlusMezera, '') != ''">
					<xsl:call-template name="substring-before-last">
						<xsl:with-param name="delimiter" select="$mezera" />
						<xsl:with-param name="string" select="." />
					</xsl:call-template>
					<xsl:value-of select="$mezera"/>
				</xsl:when>
				<xsl:otherwise>
					<xsl:apply-templates />
				</xsl:otherwise>
			</xsl:choose>
		</xsl:copy>
	</xsl:template>


	<xsl:template match="text[preceding-sibling::*[1]/self::unclear]">
		<xsl:copy>
			<xsl:copy-of select="@*" />
			<xsl:choose>
				<!-- unclear nezačíná mezerou -->
				<xsl:when test="substring(preceding-sibling::*[1], string-length(preceding-sibling::*[1]), 1) != ' ' and contains(., $mezera) and translate(substring(., 1, 1), $interpunkcePlusMezera, '') != ''">
					<xsl:variable name="znak">
					<xsl:call-template name="najitPrvniVyskytujiciSeZnak">
						<xsl:with-param name="text" select="." />
						<xsl:with-param name="interpunkce" select="$interpunkcePlusMezera" />
					</xsl:call-template>
					</xsl:variable>
					<xsl:call-template name="vlozit-text-pred-interpunkci">
						<xsl:with-param name="text" select="."/>
					</xsl:call-template>
					<xsl:call-template name="substring-after-including">
						<xsl:with-param name="text" select="." />
						<xsl:with-param name="znak" select="$znak" />
					</xsl:call-template>
<!--					<xsl:value-of select="$mezera"/>
					<xsl:value-of select="substring-after(., $mezera)"/>
-->
				</xsl:when>
				<xsl:otherwise>
					<xsl:apply-templates />
				</xsl:otherwise>
			</xsl:choose>
		</xsl:copy>
	</xsl:template>

	<xsl:template match="unclear[(preceding-sibling::*[1]/self::text or following-sibling::*[1]/self::text) and supplied[contains(normalize-space(.),' ')]]" priority="10">
		<xsl:variable name="left" select="substring-before(supplied/text(), $mezera)" />
		<xsl:variable name="right" select="substring-after(supplied/text(), $mezera)" />
		
		 <xsl:choose>
		 	<!-- předchozí text nekončí mezerou ani interpunkcí -->
		 	<xsl:when test="preceding-sibling::*[1]/self::text and translate(substring(preceding-sibling::*[1], string-length(preceding-sibling::*[1]), 1), $interpunkcePlusMezera, '') != ''">
		 		<xsl:copy>
		 			<xsl:copy-of select="@*"/>
		 			<!--<xsl:attribute name="xml:space"><xsl:text>preserve</xsl:text></xsl:attribute>-->
		 		<xsl:call-template name="substring-after-last">
		 			<xsl:with-param name="delimiter" select="$mezera" />
		 			<xsl:with-param name="string" select="preceding-sibling::*[1]" />
		 		</xsl:call-template>
		 		<supplied><xsl:value-of select="$left"/></supplied>
		 			<xsl:if test="supplied/note">
		 				<xsl:copy-of select="supplied/note"/>
		 			</xsl:if>
		 			<text><xsl:value-of select="$mezera"/></text><!-- Kvůli Altova XSLT 2.0 a zachovávání mezer u mixed elements -->
		 		</xsl:copy>
					<!--<text xml:space="preserve"><xsl:value-of select="$mezera"/></text>-->
		 		<xsl:if test="string-length($right) &gt; 0">
		 		<xsl:copy>
		 			<xsl:copy-of select="@*"/>
		 			<!--<xsl:attribute name="xml:space"><xsl:text>preserve</xsl:text></xsl:attribute>-->
		 			<supplied><xsl:value-of select="$right"/></supplied>
		 			<xsl:if
		 				test="contains(following-sibling::*[1], $mezera) and translate(substring(following-sibling::*[1], 1, 1), $interpunkcePlusMezera, '') != ''">
		 				<xsl:variable name="znak">
		 					<xsl:call-template name="najitPrvniVyskytujiciSeZnak">
		 						<xsl:with-param name="text" select="following-sibling::*[1]" />
		 						<xsl:with-param name="interpunkce" select="$interpunkcePlusMezera" />
		 					</xsl:call-template>
		 				</xsl:variable>
		 				
		 				<xsl:choose>
		 					<xsl:when test="$znak = ''" />
		 					<xsl:otherwise>
		 						<xsl:value-of select="substring-before(following-sibling::*[1], $znak)"/>
		 						<!--<text><xsl:value-of select="$znak"/></text>-->
		 					</xsl:otherwise>
		 				</xsl:choose>
		 				<!--<xsl:value-of select="substring-before(following-sibling::*[1], $mezera)"/>-->
		 			</xsl:if>
		 		</xsl:copy>
		 		</xsl:if>
		 	</xsl:when>
		 </xsl:choose>
		
		
	</xsl:template>

	<xsl:template match="unclear[preceding-sibling::*[1]/self::text or following-sibling::*[1]/self::text]">
		<xsl:copy>
			<xsl:copy-of select="@*"/>
			<xsl:choose>
				<!-- rekonstruovaný začíná mezerou nebo interpunkcí -->
				<xsl:when test="translate(substring(., 1, 1), $interpunkcePlusMezera, '') = ''">
					<xsl:apply-templates />
				</xsl:when>
				<!-- předchozí text nekončí mezerou ani interpunkcí -->
				<xsl:when test="preceding-sibling::*[1]/self::text and translate(substring(preceding-sibling::*[1], string-length(preceding-sibling::*[1]), 1), $interpunkcePlusMezera, '') != ''">
					<xsl:call-template name="substring-after-last">
						<xsl:with-param name="delimiter" select="$mezera" />
						<xsl:with-param name="string" select="preceding-sibling::*[1]" />
					</xsl:call-template>
					<xsl:apply-templates />
					<!-- unclear nekonční mezerou a následuje text -->
					<xsl:if test="substring(., string-length(.), 1) != ' ' and following-sibling::*[1]/self::text">
						<xsl:if
							test="contains(following-sibling::*[1], $mezera) and translate(substring(following-sibling::*[1], 1, 1), $interpunkcePlusMezera, '') != ''">
							<xsl:variable name="znak">
								<xsl:call-template name="najitPrvniVyskytujiciSeZnak">
									<xsl:with-param name="text" select="following-sibling::*[1]" />
									<xsl:with-param name="interpunkce" select="$interpunkcePlusMezera" />
								</xsl:call-template>
							</xsl:variable>
							
							<xsl:choose>
								<xsl:when test="$znak = ''" />
								<xsl:otherwise>
									<xsl:value-of select="substring-before(following-sibling::*[1], $znak)"/>		
								</xsl:otherwise>
							</xsl:choose>
							<!--<xsl:value-of select="substring-before(following-sibling::*[1], $mezera)"/>-->
						</xsl:if>
					</xsl:if>
				</xsl:when>
				<xsl:otherwise>
					<xsl:apply-templates />
					<xsl:if test="substring(., string-length(.), 1) != ' ' and following-sibling::*[1]/self::text and contains(following-sibling::*[1], $mezera) and translate(substring(following-sibling::*[1], 1, 1), $interpunkcePlusMezera, '') != ''">
						
						<xsl:variable name="znak">
							<xsl:call-template name="najitPrvniVyskytujiciSeZnak">
								<xsl:with-param name="text" select="following-sibling::*[1]" />
								<xsl:with-param name="interpunkce" select="$interpunkcePlusMezera" />
							</xsl:call-template>
						</xsl:variable>

						<xsl:value-of select="substring-before(following-sibling::*[1], $znak)"/>

					</xsl:if>
				</xsl:otherwise>
			</xsl:choose>
		</xsl:copy>
	</xsl:template>
	
<xd:doc>
	<xd:desc><xd:p>Najde první znak ze seznamu znaků, který se vysytuje v rámci textu. Pokud znak v textu neexistuje, vrátí prázdný řetězec.</xd:p></xd:desc>
</xd:doc>
	<xsl:template name="najitPrvniVyskytujiciSeZnak">
		<xsl:param name="text" />
		<xsl:param name="interpunkce" />
		<xsl:param name="zacatek" select="1" />
		<xsl:choose>
			<xsl:when test="string-length($text) &lt; $zacatek">
				<xsl:value-of select="''"/>
			</xsl:when>
			<xsl:when test="translate(substring($text, $zacatek, 1), $interpunkce, '') = ''" >
				<xsl:value-of select="substring($text, $zacatek, 1)"/>
			</xsl:when>
			<xsl:otherwise>
				<xsl:call-template name="najitPrvniVyskytujiciSeZnak">
					<xsl:with-param name="text" select="$text" />
					<xsl:with-param name="interpunkce" select="$interpunkce" />
					<xsl:with-param name="zacatek" select="$zacatek + 1" />
				</xsl:call-template>
			</xsl:otherwise>
		</xsl:choose>
	</xsl:template>
	
	<xsl:template match="unclear[parent::foreign][following::node()[1]/self::text()]">
		<xsl:copy>
			<xsl:copy-of select="@*" />
			<xsl:if test="contains(preceding::text()[1][parent::*/self::foreign], $mezera)">
				<xsl:call-template name="substring-after-last">
					<xsl:with-param name="string" select="preceding::text()[1]" />
					<xsl:with-param name="delimiter" select="$mezera" />
				</xsl:call-template>
			</xsl:if>
			<xsl:apply-templates /> <!-- zkopíruje se vnitřek elementu -->
			<xsl:choose>
				<xsl:when test="translate(substring(following::text()[1], 1, 1), $interpunkcePlusMezera, '') != ''">
					<xsl:choose>
						<xsl:when test="contains(following::text()[1], $mezera)">
							<xsl:value-of select="substring-before(following::text()[1], $mezera)"/>
							<xsl:value-of select="$mezera"/>
						</xsl:when>
						<xsl:otherwise>
							<xsl:value-of select="following::text()[1]"/>
						</xsl:otherwise>
					</xsl:choose>
				</xsl:when>
			</xsl:choose>
			<!--<xsl:if test="contains(following::text()[1], $mezera)">
				<xsl:value-of select="substring-before(following::text()[1], $mezera)"/>
				<xsl:value-of select="$mezera"/>
			</xsl:if>-->
		</xsl:copy>
	</xsl:template>

	<xd:doc>
		<xd:desc>
			<xd:p>Text nacházející se před prvkem <xd:b>unclear</xd:b>.</xd:p>
		</xd:desc>
	</xd:doc>
		<xsl:template match="foreign/text()[following-sibling::node()[1]/self::unclear]">
			<xsl:choose>
				<xsl:when test="contains(., $mezera) and 
					translate(substring(., 1, 1), $interpunkcePlusMezera, '') != ''">
					<xsl:call-template name="substring-before-last">
						<xsl:with-param name="string" select="." />
						<xsl:with-param name="delimiter" select="$mezera" />
					</xsl:call-template>
					<xsl:value-of select="$mezera"/>
				</xsl:when>
				<xsl:otherwise>
					<xsl:value-of select="."/>
				</xsl:otherwise>
			</xsl:choose>
	</xsl:template>
	
	<xd:doc>
		<xd:desc>
			<xd:p>Text nacházející se za prvkem <xd:b>unclear</xd:b>.</xd:p>
		</xd:desc>
	</xd:doc>
	<xsl:template match="foreign/text()[preceding-sibling::node()[1]/self::unclear]">
		<xsl:choose>
			<xsl:when test="translate(substring(., 1, 1), $interpunkcePlusMezera, '') != ''">
					<xsl:if test="contains(., $mezera)">
						<xsl:value-of select="substring-after(., $mezera)"/>
					</xsl:if>
			</xsl:when>
			<xsl:otherwise>
				<xsl:value-of select="."/>
			</xsl:otherwise>
		</xsl:choose>
	</xsl:template>

	<!--############################################################-->
	<!--## Template to determine Substring before last occurence  ##-->
	<!--## of a specific delemiter                                ##-->
	<!--## http://www.heber.it/?p=1278                            ##-->
	<!--############################################################-->
	<xsl:template name="substring-before-last">
		<!--passed template parameter -->
		<xsl:param name="string"/>
		<xsl:param name="delimiter"/>
		<xsl:choose>
			<xsl:when test="contains($string, $delimiter)">
				<!-- get everything in front of the first delimiter -->
				<xsl:value-of select="substring-before($string,$delimiter)"/>
				<xsl:choose>
					<xsl:when test="contains(substring-after($string,$delimiter),$delimiter)">
						<xsl:value-of select="$delimiter"/>
					</xsl:when>
				</xsl:choose>
				<xsl:call-template name="substring-before-last">
					<!-- store anything left in another variable -->
					<xsl:with-param name="string" select="substring-after($string,$delimiter)"/>
					<xsl:with-param name="delimiter" select="$delimiter"/>
				</xsl:call-template>
			</xsl:when>
		</xsl:choose>
	</xsl:template>

	<xsl:template name="substring-after-last">
		<xsl:param name="string" />
		<xsl:param name="delimiter" />
		<xsl:choose>
			<xsl:when test="contains($string, $delimiter)">
				<xsl:call-template name="substring-after-last">
					<xsl:with-param name="string"
						select="substring-after($string, $delimiter)" />
					<xsl:with-param name="delimiter" select="$delimiter" />
				</xsl:call-template>
			</xsl:when>
			<xsl:otherwise><xsl:value-of select="$string" /></xsl:otherwise>
		</xsl:choose>
	</xsl:template>
	
	<xd:doc>
		<xd:desc><xd:p>Vloží text za hledaným řetězcem včetně hledaného řetězce.</xd:p></xd:desc>
	</xd:doc>
	<xsl:template name="substring-after-including">
		<xsl:param name="text" />
		<xsl:param name="znak" />
		<xsl:choose>
			<xsl:when test="substring-after($text, $znak) = ''">
				<xsl:value-of select="''"/>
			</xsl:when>
			<xsl:when test="starts-with($text, $znak)">
					<xsl:value-of select="$text"/>
			</xsl:when>
			<xsl:otherwise>
				<xsl:value-of select="substring($text,  string-length($znak) + string-length(substring-before($text, $znak)))"/>
			</xsl:otherwise>
		</xsl:choose>
	</xsl:template>
	
	<xd:doc>
		<xd:desc><xd:p>Vloží text za hledaným řetězcem včetně hledaného řetězce.</xd:p></xd:desc>
	</xd:doc>
	<xsl:template name="substring-before-including">
		<xsl:param name="text" />
		<xsl:param name="znak" />
		<xsl:choose>
			<xsl:when test="substring-before($text, $znak) = ''">
				<xsl:value-of select="''"/>
			</xsl:when>
<!--			<xsl:when test="substring($text, string-length($text) - string-length($znak) +1) = $znak">
				<xsl:value-of select="$text"/>
			</xsl:when>-->
			<xsl:otherwise>
				<xsl:value-of select="concat(substring-before($text, $znak), $znak)"/>
			</xsl:otherwise>
		</xsl:choose>
	</xsl:template>
	
</xsl:stylesheet>
