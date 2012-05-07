//
//  ViewController.m
//  UploadTest
//
//  Created by Jackson Harper on 5/6/12.
//  Copyright (c) 2012 Entrada Health. All rights reserved.
//

#import "ViewController.h"

@implementation ViewController

- (void)didReceiveMemoryWarning
{
    [super didReceiveMemoryWarning];
    // Release any cached data, images, etc that aren't in use.
}

#pragma mark - View lifecycle

- (void)viewDidLoad
{
    [super viewDidLoad];
	// Do any additional setup after loading the view, typically from a nib.
}

- (void)viewDidUnload
{
    [super viewDidUnload];
    // Release any retained subviews of the main view.
    // e.g. self.myOutlet = nil;
}

- (void)viewWillAppear:(BOOL)animated
{
    [super viewWillAppear:animated];
}

- (void)viewDidAppear:(BOOL)animated
{
    [super viewDidAppear:animated];
}

- (void)viewWillDisappear:(BOOL)animated
{
	[super viewWillDisappear:animated];
}

- (void)viewDidDisappear:(BOOL)animated
{
	[super viewDidDisappear:animated];
}

- (BOOL)shouldAutorotateToInterfaceOrientation:(UIInterfaceOrientation)interfaceOrientation
{
    // Return YES for supported orientations
    return (interfaceOrientation != UIInterfaceOrientationPortraitUpsideDown);
}

- (void)uploadToURL:(NSURL *)url
{
	NSMutableURLRequest *request = [[NSMutableURLRequest alloc] initWithURL:url 
                                                  cachePolicy:NSURLRequestReturnCacheDataElseLoad timeoutInterval:10.0];
    
    request.HTTPMethod = @"POST";
    
    char *array = malloc(5242880);
    request.HTTPBody = [NSData dataWithBytes:array length:5242880]; 
    
    [request setValue:@"jacksonh" forHTTPHeaderField:@"dictator"];
    [request setValue:@"testfile.data" forHTTPHeaderField:@"filename"];
    [request setValue:@"5242880" forHTTPHeaderField:@"Content-Length"];
    
	_connection = [[NSURLConnection alloc] initWithRequest:request delegate:self 
                                          startImmediately:NO];
	
    [_connection scheduleInRunLoop:[NSRunLoop currentRunLoop] 
                           forMode:NSRunLoopCommonModes];
	[_connection start];

}

- (IBAction)OnUploadClicked:(id)sender {

    NSURL *url = [NSURL URLWithString:@"http://ec.jacksonh.org:9000/"];
    
    [self uploadToURL:url];

}

-(BOOL)connection:(NSURLConnection *)connection canAuthenticateAgainstProtectionSpace:(NSURLProtectionSpace *)protectionSpace
{
    NSLog (@"  canAuthenticateAgainstProtectionSpace:");
    return YES;
}

-(void)connection:(NSURLConnection *)connection didReceiveAuthenticationChallenge:(NSURLAuthenticationChallenge *)challenge
{
    if ([challenge.protectionSpace.authenticationMethod isEqualToString:NSURLAuthenticationMethodServerTrust])
        [challenge.sender useCredential:[NSURLCredential credentialForTrust:challenge.protectionSpace.serverTrust] forAuthenticationChallenge:challenge];
}

-(void)connection:(NSURLConnection *)connection didReceiveResponse:(NSURLResponse *)response
{
    NSHTTPURLResponse *httpres = (NSHTTPURLResponse *) response;
    NSLog (@"  response code: %d", httpres.statusCode);
    
}

-(void)connection:(NSURLConnection *)connection didFailWithError:(NSError *)error
{
    NSLog (@"  failed with error: %@", error.localizedDescription);
}



@end
