<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" 
	xmlns:xd="http://www.oxygenxml.com/ns/doc/xsl"
	xmlns:xml="http://www.w3.org/XML/1998/namespace"
	xmlns:vwf="http://vokabular.ujc.cas.cz/xslt/functions"
	xmlns:xs="http://www.w3.org/2001/XMLSchema"
	xmlns:fn="http://www.w3.org/2005/xpath-functions"
	extension-element-prefixes="vwf fn"
	exclude-result-prefixes="xd vwf xs" version="2.0"
	>
	<xsl:template match="*[following-sibling::node()[1]/self::unclear]" priority="10">

		<!--<xsl:variable name="current-text" select="string(.)" />-->
		<xsl:variable name="current-text">
			<xsl:copy-of select="text()" />
		</xsl:variable>
		
		<xsl:choose>
			<xsl:when test="not[node()]">
				<!-- případy typu pb -->
				<xsl:copy-of select="." />
			</xsl:when>
			<xsl:otherwise>
				<xsl:variable name="start" as="xs:integer">
					<xsl:choose>
						<xsl:when test="preceding-sibling::*[1]/self::unclear and not(vwf:ends-with(preceding-sibling::*[1], $mezera))">
							<xsl:value-of select="vwf:first-position-of-many($current-text, $interpunkcePlusMezera)" />
						</xsl:when>
						<xsl:otherwise>
							<xsl:value-of select="number(1)" />
						</xsl:otherwise>
					</xsl:choose>
				</xsl:variable>
				
				<xsl:variable name="next-text">
					<xsl:copy-of select="following-sibling::*[1]/supplied/text()" />
				</xsl:variable>
				<!-- bude vždy false -->
				<xsl:variable name="next-startsWithSpace"><xsl:value-of select="starts-with($next-text, $mezera)" /></xsl:variable>
				<xsl:variable name="next-endsWithSpace"><xsl:value-of select="vwf:ends-with($next-text, $mezera)" /></xsl:variable>
				<xsl:variable name="next-containsSpace"><xsl:value-of select="contains(normalize-space($next-text), $mezera)" /></xsl:variable>
				
				<xsl:variable name="end" as="xs:integer">
					<xsl:value-of select="vwf:last-position-of-many($current-text, $interpunkcePlusMezera)" />
				</xsl:variable>
				
				<xsl:variable name="length" as="xs:integer">
					<xsl:value-of select="$end + 1 - $start" />
				</xsl:variable>
				
				<xsl:message> $next-text: <xsl:value-of select="concat('[', $next-text, ']')" />, $current-text: <xsl:value-of select="concat('[', $current-text, ']')" />,  $start: <xsl:value-of select="$start" />, $end: <xsl:value-of select="$end" />, $length: <xsl:value-of select="$length" />, result: <xsl:value-of select="concat('[', substring($current-text,  $start, $length), ']')" /> </xsl:message>
				
				
				<xsl:copy>
					<xsl:copy-of select="@*" />
					<xsl:choose>
						<xsl:when test="note">
							<xsl:variable name="left-text">
								<xsl:copy-of select="text()[not(preceding-sibling::*/self::note)]"></xsl:copy-of>
							</xsl:variable>
							<xsl:variable name="right-text">
								<xsl:copy-of select="text()[not(following-sibling::*/self::note) and not(self::note)]"></xsl:copy-of>
							</xsl:variable>
							
							<xsl:value-of select="substring($left-text, $start, $length)" />
							<xsl:copy-of select="note" />
							<xsl:if test="string-length($left-text) &lt; $length">
								<xsl:value-of select="substring($right-text, 1, $length - string-length($left-text) ) "></xsl:value-of>
							</xsl:if>
						</xsl:when>
					<xsl:otherwise>
						<xsl:if test="substring($current-text, $start, $length) = $mezera">
							<xsl:attribute name="xml:space" select="'preserve'" />
						</xsl:if>
						<xsl:value-of select="substring($current-text, $start, $length)" />
					</xsl:otherwise>
				</xsl:choose>
				</xsl:copy>
				
			</xsl:otherwise>
		</xsl:choose>
		
	</xsl:template>
	
	
	<xsl:template match="*[preceding-sibling::*[1]/self::unclear]">
		<!-- pokud předchozí element (unclear) nekončí na mezeru, musím odstranit text před interpunkcí nebo mezerou  -->
		<xsl:variable name="prev-text" select="string(preceding-sibling::*[1])" />
		<xsl:variable name="current-text">
			<xsl:copy-of select="text()" />
		</xsl:variable>
		
		<xsl:choose>
			<xsl:when test="not(node())">
				<xsl:copy-of select="." />
			</xsl:when>
			<xsl:otherwise>
				
					<xsl:variable name="start">
						<xsl:choose>
							<xsl:when test="vwf:ends-with($prev-text, $mezera)">
								<xsl:value-of select="number(1)" />
							</xsl:when>
							<xsl:otherwise>
								<xsl:value-of select="vwf:first-position-of-many($current-text, $interpunkcePlusMezera)" />
							</xsl:otherwise>
						</xsl:choose>
					</xsl:variable>
				
				
					<xsl:variable name="end">
						<xsl:choose>
							<xsl:when test="following-sibling::*[1]/self::unclear">
								<xsl:value-of select="vwf:last-position-of-many($current-text, $mezera)" />
							</xsl:when>
							<xsl:otherwise>
								<xsl:value-of select="string-length($current-text)" />
							</xsl:otherwise>
						</xsl:choose>
					</xsl:variable>
				
				<xsl:variable name="length" select="$end + 1 - $start" />
				
				<xsl:message> $prev-text: <xsl:value-of select="concat('[', $prev-text, ']')" />, $current-text: <xsl:value-of select="concat('[', $current-text, ']')" />,  $start: <xsl:value-of select="$start" />, $end: <xsl:value-of select="$end" />, $length: <xsl:value-of select="$length" />, result: <xsl:value-of select="concat('[', substring($current-text,  $start, $length), ']')" /> </xsl:message>
				
				<xsl:copy>
					<xsl:copy-of select="@*" />
					
					<xsl:choose>
						<xsl:when test="note">
							<xsl:variable name="left-text">
								<xsl:copy-of select="text()[not(preceding-sibling::*/self::note)]"></xsl:copy-of>
							</xsl:variable>
							<xsl:variable name="right-text">
								<xsl:copy-of select="text()[not(following-sibling::*/self::note) and not(self::note)]"></xsl:copy-of>
							</xsl:variable>
							
							<xsl:value-of select="substring($left-text, $start, $length)" />
							<xsl:copy-of select="note" />
							<xsl:if test="string-length($left-text) &lt; $length">
								<xsl:value-of select="substring($right-text, 1, $length - string-length($left-text) + $start) "></xsl:value-of>
							</xsl:if>
							
							
						<!--	<xsl:value-of select="substring($current-text,  $start, $length)" />-->
						</xsl:when>
						<xsl:otherwise>
							<xsl:if test="substring($current-text, $start, $length) = $mezera">
								<xsl:attribute name="xml:space" select="'preserve'" />
							</xsl:if>
							<xsl:value-of select="substring($current-text,  $start, $length)" />
						</xsl:otherwise>
					</xsl:choose>
					
					
				</xsl:copy>
				
			</xsl:otherwise>
		</xsl:choose>
	</xsl:template>
	
	
	
	<xsl:template match="unclear">
		
		
		<xsl:variable name="current-text">
			<xsl:copy-of select="supplied/text()" />
		</xsl:variable>
		
		<xsl:variable name="previous-text">

			<xsl:variable name="source-previous-text">
				<xsl:choose>
					<xsl:when test="preceding-sibling::*[1]/self::text or preceding-sibling::*[1]/self::supplied">
						<xsl:value-of select="string(preceding-sibling::*[1])" />
					</xsl:when>
					<xsl:when test="preceding-sibling::node()[1]/self::text()">
						<xsl:value-of select="preceding-sibling::node()[1]" />
					</xsl:when>
					<xsl:otherwise>
						<xsl:value-of select="''" />
					</xsl:otherwise>
				</xsl:choose>
			</xsl:variable>
			
			<xsl:choose>
				<xsl:when test="vwf:ends-with-one-of-character($source-previous-text, $interpunkcePlusMezera)">
					<xsl:value-of select="''" />
				</xsl:when>
				<xsl:otherwise>
					<xsl:variable name="prev-text" select="string($source-previous-text)" />
					<xsl:choose>
						<xsl:when test="contains($prev-text, $mezera)">
							<xsl:value-of select="vwf:substring-after-last-from-many($prev-text, $mezera)" />		
						</xsl:when>
						<xsl:otherwise>
							<xsl:value-of select="$prev-text" />
						</xsl:otherwise>
					</xsl:choose>
				</xsl:otherwise>
			</xsl:choose>
<!--			
			<xsl:choose>
				<xsl:when test="preceding-sibling::*[1]/self::text or preceding-sibling::*[1]/self::supplied">
					<xsl:choose>
						<!-\-<xsl:when test="ends-with(preceding-sibling::*[1], $mezera)">-\->
						<xsl:when test="vwf:ends-with-one-of-character(preceding-sibling::*[1], $interpunkcePlusMezera)">
							<xsl:value-of select="''" />
						</xsl:when>
						<xsl:otherwise>
							<xsl:variable name="prev-text" select="string(preceding-sibling::*[1])" />
							<xsl:choose>
								<xsl:when test="contains($prev-text, $mezera)">
									<xsl:value-of select="vwf:substring-after-last-from-many($prev-text, $mezera)" />		
								</xsl:when>
								<xsl:otherwise>
									<xsl:value-of select="$prev-text" />
								</xsl:otherwise>
							</xsl:choose>
						</xsl:otherwise>
					</xsl:choose>
				</xsl:when>
				<xsl:when test="preceding-sibling::node()[1]/self::text()">
					<xsl:value-of select="preceding-sibling::node()[1]" />
				</xsl:when>
				<xsl:otherwise>
					<xsl:value-of select="''" />
				</xsl:otherwise>
			</xsl:choose>
-->

		</xsl:variable>
		
		<xsl:variable name="source-next-text">
			<xsl:choose>
				<xsl:when test="following-sibling::*[1]/self::text or following-sibling::*[1]/self::supplied">
					<xsl:value-of select="following-sibling::*[1]" />
				</xsl:when>
				<xsl:when test="following-sibling::node()[1]/self::text()">
					<xsl:value-of select="following-sibling::node()[1]" />
				</xsl:when>
				<xsl:otherwise>
					<xsl:value-of select="''" />
				</xsl:otherwise>
			</xsl:choose>
		</xsl:variable>
		
		<xsl:variable name="next-text">
			<xsl:choose>
				<xsl:when test="ends-with($current-text, $mezera)">
					<xsl:value-of select="''" />
				</xsl:when>
				<xsl:otherwise>
					<xsl:choose>
						<xsl:when test="vwf:starts-with-one-from-many($source-next-text, $interpunkcePlusMezera)">
							<xsl:value-of select="''" />
						</xsl:when>
						<xsl:otherwise>
							<xsl:variable name="znak">
								<xsl:call-template name="najitPrvniVyskytujiciSeZnak">
									<xsl:with-param name="text" select="$source-next-text" />
									<xsl:with-param name="interpunkce" select="$interpunkcePlusMezera" />
								</xsl:call-template>
							</xsl:variable>
							<xsl:value-of select="substring-before($source-next-text, $znak)" />
						</xsl:otherwise>
					</xsl:choose>
				</xsl:otherwise>
			</xsl:choose>
		</xsl:variable>
		
		
	<!--	
		<xsl:variable name="next-text">
			<xsl:choose>
				<xsl:when test="ends-with($current-text, $mezera)">
					<xsl:value-of select="''" />
				</xsl:when>
				<xsl:when test="following-sibling::*[1]/self::text or following-sibling::*[1]/self::supplied">
					<xsl:variable name="nx-text" select="string(following-sibling::*[1])" />
<!-\-					<xsl:message select="concat('[', $nx-text, ']')" />-\->
					<xsl:choose>
						
						<xsl:when test="vwf:starts-with-one-from-many($nx-text, $interpunkcePlusMezera)">
							<xsl:value-of select="''" />
						</xsl:when>
						<xsl:otherwise>
							<xsl:variable name="znak">
								<xsl:call-template name="najitPrvniVyskytujiciSeZnak">
									<xsl:with-param name="text" select="$nx-text" />
									<xsl:with-param name="interpunkce" select="$interpunkcePlusMezera" />
								</xsl:call-template>
							</xsl:variable>
							<xsl:value-of select="substring-before($nx-text, $znak)" />
						</xsl:otherwise>
					</xsl:choose>
				</xsl:when>
				<xsl:when test="following-sibling::node()[1]/self::text()">
					<xsl:value-of select="following-sibling::node()[1]" />
				</xsl:when>
			</xsl:choose>
		</xsl:variable>-->
		
		<xsl:choose>
			<xsl:when test="preceding-sibling::*[1][not(node())]">
				<xsl:copy-of select="."></xsl:copy-of>
			</xsl:when>
			<xsl:when test="contains(normalize-space($current-text), $mezera)">
				 <xsl:copy><xsl:copy-of select="@*" /><xsl:value-of select="$previous-text" /><supplied><xsl:value-of select="concat(substring-before($current-text, $mezera), $mezera)" /></supplied></xsl:copy>
					<xsl:copy><xsl:copy-of select="@*" /><supplied><xsl:value-of select="substring-after($current-text, $mezera)" /></supplied><xsl:value-of select="$next-text" /></xsl:copy>
			</xsl:when>
			<xsl:otherwise>
				<xsl:copy><xsl:copy-of select="@*" /><xsl:value-of select="$previous-text" /><xsl:copy-of select="supplied" /><xsl:value-of select="$next-text" /></xsl:copy>
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



	<xsl:template name="zpacuj-unclear">
		<xsl:param name="node" as="element()" />
		<xsl:param name="previous-text" as="xs:string" />
		<xsl:variable name="text" as="xs:string" select="string($node/supplied)" />
		
		<xsl:choose>
			<xsl:when test="contains(normalize-space($node/supplied), $mezera)">
				<xsl:copy>
					<xsl:value-of select="$previous-text"/><supplied><xsl:value-of select="substring-before($text, $mezera)"/></supplied>
				</xsl:copy>
				<xsl:copy>
					<supplied><xsl:value-of select="substring-after($text, $mezera)"/></supplied>
					<!-- text z následujícího elementu -->
				</xsl:copy>
			</xsl:when>
			<xsl:otherwise>
				<xsl:copy>
					<xsl:value-of select="$previous-text"/><supplied><xsl:value-of select="substring-before($text, $mezera)"/></supplied>
				</xsl:copy>
			</xsl:otherwise>
		</xsl:choose>
		
		<xsl:copy>
			<xsl:choose>
				<xsl:when test="string-length($previous-text) = 0">
					
				</xsl:when>
			</xsl:choose>
		</xsl:copy>
	</xsl:template>
</xsl:stylesheet>