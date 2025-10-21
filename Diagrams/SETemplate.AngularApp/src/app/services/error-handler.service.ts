//@CodeCopy
import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class ErrorHandlerService {

  /**
   * Extracts detailed error information from HTTP error responses.
   * Specifically handles BusinessRuleExceptions and formats them nicely for display.
   * 
   * @param error - The error object from HTTP response
   * @returns Formatted error details string
   */
  public extractErrorDetails(error: any): string {
    // Zuerst versuchen, die BusinessRule-Exception zu finden
    if (error && error.error) {
      const errorText = error.error;
      
      // Pattern für BusinessRuleException
      const businessRuleMatch = errorText.match(/\[0\]\s*BusinessRuleException:\s*(.+?)(?:\n|$)/) ||
                               errorText.match(/BusinessRuleException:\s*(.+?)(?:\n|$)/);
      
      if (businessRuleMatch) {
        // BusinessRule als Hauptinhalt + technische Details
        const businessRule = businessRuleMatch[1].trim();
        const technicalDetails = [];
        
        if (error.status) technicalDetails.push(`Status: ${error.status} ${error.statusText}`);
        if (error.url) technicalDetails.push(`URL: ${error.url}`);
        
        return `${businessRule}\n\n--- Technische Details ---\n${technicalDetails.join('\n')}\n\nVollständiger Fehler:\n${errorText}`;
      }
      
      // Fallback: kompletter error.error Text
      return errorText;
    }
    
    // Fallback für andere Fehlertypen
    const details = [];
    if (error.status) details.push(`Status: ${error.status} ${error.statusText}`);
    if (error.url) details.push(`URL: ${error.url}`);
    if (error.message) details.push(`Message: ${error.message}`);
    
    return details.length > 0 ? details.join('\n') : error.toString();
  }
}
