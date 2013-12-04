These are a modified set of Amazon on-guest CFN helper scripts that have been modified to work in a 
non-AWS environment, by the addition of a adaptor layer to abstract any environment specifics (URLs etc): see adaptor.py 
in the cfnbootstrap folder
Also see http://docs.aws.amazon.com/AWSCloudFormation/latest/UserGuide/cfn-helper-scripts-reference.html for original tools 
reference.

Build instructions for Windows:

0. (modify adaptor.py for you environment)
1. Install 64 bit Windows Python 2.7 (e.g. http://www.python.org/download/releases/2.7.6/)
2, Install required packages using pip: certifi, chardet, requests, py2exe, pywin32
3. Build the executables using command: python setup.py py2exe