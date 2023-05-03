import { observer } from 'mobx-react-lite'
import React, { useEffect } from 'react'
import {Segment, Header, Comment, Button} from 'semantic-ui-react'
import { useStore } from '../../../app/strores/store';
import { Link } from 'react-router-dom';
import { Formik, Form } from 'formik';
import MyTextArea from '../../../app/common/form/MyTextArea';
import * as Yup from "yup"
import { formatDistanceToNow } from 'date-fns';

interface Props {
    activityId: string;
}

export default observer(function ActivityDetailedChat({ activityId }: Props) {
    const { commentStore } = useStore();

    useEffect(() => {
        if (activityId) {
            commentStore.createHubConnection(activityId);
        }
        return () => {
            commentStore.clearCommetns();
        }
    }, [commentStore, activityId]);
    
    return (
        <>
            <Segment
                textAlign='center'
                attached='top'
                inverted
                color='teal'
                style={{border: 'none'}}
            >
                <Header>Chat about this event</Header>
            </Segment>
            <Segment attached clearing>
                <Formik
                    validationSchema={Yup.object({
                        body: Yup.string().required()
                    })}
                    initialValues={{body: ''}}
                    onSubmit={(values: any, {resetForm}) => commentStore.addComment(values).catch(error => console.log(error)).then(() => resetForm())}>
                    {({ isSubmitting, isValid  }) => (
                        <Form className='ui form'>
                            <MyTextArea placeholder='Add comments' name='body' rows={2}/>
                            <Button
                                loading={isSubmitting}
                                disabled={isSubmitting || !isValid}
                                content='Add Reply'
                                labelPosition='left'
                                icon='edit'
                                primary
                                type='submit'
                                floated='right'
                            />
                        </Form>
                        )}
                </Formik>
                <Comment.Group>
                    {commentStore.comments && commentStore.comments.map(comment => (
                        <Comment key={comment.id}>
                            <Comment.Avatar src={comment.image || '/assets/user.png'}/>
                            <Comment.Content>
                                <Comment.Author as={Link} to={`/profiles/${comment.username}`}>{comment.displayName}</Comment.Author>
                                <Comment.Metadata>
                                    <div>{formatDistanceToNow(comment.createdAt)} ago</div>
                                </Comment.Metadata>
                                <Comment.Text style={{whiteSpace: 'pre-wrap'}}>{comment.body}</Comment.Text>
                            </Comment.Content>
                        </Comment>
                    ))}
                    

                    
                </Comment.Group>
            </Segment>
        </>

    )
})
