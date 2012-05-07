//
//  ViewController.h
//  UploadTest
//
//  Created by Jackson Harper on 5/6/12.
//  Copyright (c) 2012 Entrada Health. All rights reserved.
//

#import <UIKit/UIKit.h>

@interface ViewController : UIViewController {
    
    NSURLConnection *_connection;
}

- (IBAction)OnUploadClicked:(id)sender;
@end
